using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Interfaces.Externals;

namespace SylviaNG.Community.Infrastructure.Authentication
{
    /// <summary>
    /// Calls Keycloak's Admin REST API (https://www.keycloak.org/docs-api/latest/rest-api/index.html)
    /// to provision real Keycloak users. Distinct from JwtTokenGenerator, which issues this
    /// service's own local JWTs and never talks to Keycloak at all.
    ///
    /// Derives the Keycloak server root and realm from Keycloak:Authority (".../realms/{realm}").
    /// Uses Keycloak:AdminClientId/AdminClientSecret if configured, otherwise falls back to the
    /// existing Keycloak:ClientId/ClientSecret (used elsewhere only to validate incoming tokens) -
    /// a dedicated least-privilege service-account client is recommended for production, but reusing
    /// the existing one lets this work without a prerequisite realm change.
    ///
    /// The configured service account must have the realm-management "manage-users" role (and
    /// permission to manage realm role mappings) granted in Keycloak - this is realm-side
    /// configuration this service cannot verify or grant; a 401/403 from Keycloak is surfaced as a
    /// clear ExternalServiceException rather than silently failing.
    /// </summary>
    public class KeycloakAdminClient : IKeycloakAdminClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeycloakAdminClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        public KeycloakAdminClient(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakAdminClient> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> CreateUserAsync(string username, string? email, string firstName, string lastName, string temporaryPassword)
        {
            var (serverRoot, realm) = GetServerRootAndRealm();
            var adminToken = await GetAdminAccessTokenAsync(serverRoot, realm);

            var payload = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                emailVerified = false,
                credentials = new[]
                {
                    new { type = "password", value = temporaryPassword, temporary = true }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{serverRoot}/admin/realms/{realm}/users")
            {
                Content = JsonContent.Create(payload, options: JsonOptions)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            using var response = await SendAsync(request, serverRoot);

            if (response.StatusCode == HttpStatusCode.Conflict)
                throw new DuplicateException("EmployeeKeycloakAccount", "Username or email", username);

            EnsureAdminPermission(response, "create the Keycloak user");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak user creation failed ({StatusCode}): {Body}", response.StatusCode, body);
                throw new ExternalServiceException($"Keycloak rejected user creation ({(int)response.StatusCode} {response.StatusCode}).");
            }

            // Keycloak returns 201 Created with a Location header ending in the new user's ID,
            // rather than the created representation in the response body.
            var location = response.Headers.Location
                ?? throw new ExternalServiceException("Keycloak did not return a Location header for the created user.");

            return location.Segments[^1].TrimEnd('/');
        }

        public async Task AssignRealmRoleAsync(string keycloakUserId, string roleName)
        {
            var (serverRoot, realm) = GetServerRootAndRealm();
            var adminToken = await GetAdminAccessTokenAsync(serverRoot, realm);

            using var roleRequest = new HttpRequestMessage(HttpMethod.Get, $"{serverRoot}/admin/realms/{realm}/roles/{Uri.EscapeDataString(roleName)}");
            roleRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
            using var roleResponse = await SendAsync(roleRequest, serverRoot);

            if (roleResponse.StatusCode == HttpStatusCode.NotFound)
                throw new ExternalServiceException($"Keycloak realm role \"{roleName}\" does not exist. Create it in Keycloak before assigning it here.");

            EnsureAdminPermission(roleResponse, "look up the Keycloak realm role");

            if (!roleResponse.IsSuccessStatusCode)
                throw new ExternalServiceException($"Keycloak rejected the realm role lookup ({(int)roleResponse.StatusCode} {roleResponse.StatusCode}).");

            var role = await roleResponse.Content.ReadFromJsonAsync<KeycloakRole>(JsonOptions)
                ?? throw new ExternalServiceException("Keycloak returned an empty realm role representation.");

            using var mapRequest = new HttpRequestMessage(HttpMethod.Post, $"{serverRoot}/admin/realms/{realm}/users/{keycloakUserId}/role-mappings/realm")
            {
                Content = JsonContent.Create(new[] { role }, options: JsonOptions)
            };
            mapRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
            using var mapResponse = await SendAsync(mapRequest, serverRoot);

            EnsureAdminPermission(mapResponse, "assign the Keycloak realm role");

            if (!mapResponse.IsSuccessStatusCode)
            {
                var body = await mapResponse.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak role assignment failed ({StatusCode}): {Body}", mapResponse.StatusCode, body);
                throw new ExternalServiceException($"Keycloak rejected the realm role assignment ({(int)mapResponse.StatusCode} {mapResponse.StatusCode}).");
            }
        }

        public async Task ResetPasswordAsync(string keycloakUserId, string newTemporaryPassword)
        {
            var (serverRoot, realm) = GetServerRootAndRealm();
            var adminToken = await GetAdminAccessTokenAsync(serverRoot, realm);

            using var request = new HttpRequestMessage(HttpMethod.Put, $"{serverRoot}/admin/realms/{realm}/users/{keycloakUserId}/reset-password")
            {
                Content = JsonContent.Create(new { type = "password", value = newTemporaryPassword, temporary = true }, options: JsonOptions)
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);

            using var response = await SendAsync(request, serverRoot);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new NotFoundException("KeycloakUser", keycloakUserId);

            EnsureAdminPermission(response, "reset the Keycloak user's password");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak password reset failed ({StatusCode}): {Body}", response.StatusCode, body);
                throw new ExternalServiceException($"Keycloak rejected the password reset ({(int)response.StatusCode} {response.StatusCode}).");
            }
        }

        private async Task<string> GetAdminAccessTokenAsync(string serverRoot, string realm)
        {
            var adminClientId = _configuration["Keycloak:AdminClientId"] ?? _configuration["Keycloak:ClientId"]
                ?? throw new InvalidOperationException("Keycloak:AdminClientId (or Keycloak:ClientId) is not configured.");
            var adminClientSecret = _configuration["Keycloak:AdminClientSecret"] ?? _configuration["Keycloak:ClientSecret"]
                ?? throw new InvalidOperationException("Keycloak:AdminClientSecret (or Keycloak:ClientSecret) is not configured.");

            var tokenUrl = $"{serverRoot}/realms/{realm}/protocol/openid-connect/token";
            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = adminClientId,
                ["client_secret"] = adminClientSecret
            };

            using var response = await SendAsync(() => _httpClient.PostAsync(tokenUrl, new FormUrlEncodedContent(form)), serverRoot);

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new ExternalServiceException(
                    "Keycloak rejected the admin service-account credentials. The configured client " +
                    "(Keycloak:AdminClientId/ClientId) needs a service account with the \"manage-users\" " +
                    "realm-management role granted in Keycloak.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak admin token request failed ({StatusCode}): {Body}", response.StatusCode, body);
                throw new ExternalServiceException($"Could not obtain a Keycloak admin token ({(int)response.StatusCode} {response.StatusCode}).");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(JsonOptions)
                ?? throw new ExternalServiceException("Keycloak returned an empty token response.");

            return tokenResponse.AccessToken;
        }

        private (string ServerRoot, string Realm) GetServerRootAndRealm()
        {
            var authority = _configuration["Keycloak:Authority"]
                ?? throw new InvalidOperationException("Keycloak:Authority is not configured.");

            const string realmsSegment = "/realms/";
            var realmsIndex = authority.IndexOf(realmsSegment, StringComparison.OrdinalIgnoreCase);
            if (realmsIndex < 0)
                throw new InvalidOperationException($"Keycloak:Authority (\"{authority}\") is not in the expected \"{{server}}/realms/{{realm}}\" shape.");

            var serverRoot = authority[..realmsIndex];
            var realm = authority[(realmsIndex + realmsSegment.Length)..].Trim('/');
            return (serverRoot, realm);
        }

        /// <summary>
        /// A response status Keycloak itself returned (handled, and reported, elsewhere in this
        /// file) is a very different failure from the server being unreachable altogether - the
        /// latter throws HttpRequestException/TaskCanceledException straight out of HttpClient with
        /// no useful message, so every outbound call is routed through here to turn that into the
        /// same clear ExternalServiceException the rest of this client already uses.
        /// </summary>
        private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, string serverRoot) =>
            SendAsync(() => _httpClient.SendAsync(request), serverRoot);

        private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> send, string serverRoot)
        {
            try
            {
                return await send();
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                _logger.LogError(ex, "Could not reach the Keycloak server at {ServerRoot}.", serverRoot);
                throw new ExternalServiceException($"Could not reach the Keycloak server at {serverRoot}. Check it's running and reachable.");
            }
        }

        private static void EnsureAdminPermission(HttpResponseMessage response, string action)
        {
            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                throw new ExternalServiceException(
                    $"Keycloak denied permission to {action}. The configured admin service account needs " +
                    "the \"manage-users\" realm-management role (and realm role-mapping rights) in Keycloak.");
            }
        }

        private class KeycloakTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;
        }

        private class KeycloakRole
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }
    }
}
