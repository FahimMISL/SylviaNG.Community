# Keycloak realm setup for real employee login

This is a manual checklist for whoever administers the Keycloak instance backing this system (realm **`sylviang`**, currently configured at `http://localhost:8082` per `SylviaNG.Community/appsettings.json`). None of this can be done from the application codebase — it's realm/client configuration in Keycloak itself.

**How this login works:** the admin UI has a single login form (username + password), unchanged from before. On submit, the backend (`AuthService.LoginAsync`) tries the local demo-account store first, then — if that fails — calls Keycloak's token endpoint directly using a **Direct Access Grant** (`grant_type=password`) with the existing `sylviang-api` client. This is a server-to-server call (`KeycloakAdminClient.TryPasswordLoginAsync`), so `sylviang-api`'s client secret never reaches the browser — there's no need for a separate public SPA client, redirect URIs, or a second login button; the same request the frontend already sends is enough.

## 1. Enable Direct Access Grants on `sylviang-api`

Realm `sylviang` → Clients → `sylviang-api` → Settings → Capability config: turn **Direct access grants** ON. Without this, Keycloak rejects the login attempt with `unauthorized_client` and the app falls back to "Invalid username or password." for every real employee, even with the correct password.

## 2. Add protocol mappers to `sylviang-api`

Client scopes tab → `sylviang-api`'s dedicated scope → Add mapper → By configuration. Add four mappers:

| Mapper type | Config | Why |
|---|---|---|
| **Audience** | Included Client Audience: `sylviang-api`, Add to access token: ON | **Confirmed by live testing that this is required, correcting an earlier assumption in this doc.** A Direct Access Grant token issued by this realm carries `aud: "account"` (Keycloak's built-in default) unless a client is explicitly added to the audience — even though the token's `azp` (authorized party) correctly shows `sylviang-api`. The backend's `TokenValidationParameters.ValidAudience` is `sylviang-api` (`AuthenticationExtensions.cs`), so without this mapper every protected API call 401s with "The audience '(null)' is invalid" for real Keycloak logins, even though login itself succeeds. |
| **User Realm Role** | Token Claim Name: `role`, Multivalued: **ON**, Add to access token: ON | `AuthorizationExtensions.cs`'s `HRAdminOnly` policy calls `RequireRole("HR","Admin")`, which checks `ClaimTypes.Role` — ASP.NET's default inbound claim mapping auto-converts a raw `role` claim to `ClaimTypes.Role` (no `MapInboundClaims = false` override exists in this backend, confirmed). |
| **User Attribute** | User Attribute: `employee_id`, Token Claim Name: `employee_id`, Add to access token: ON | `CurrentUserService.EmployeeId` reads `FindFirst("employee_id")` with zero fallback (a middleware fallback also now resolves this from `EmployeeKeycloakAccount` via the token's `sub` claim if this mapper's claim is ever missing/dropped - see `EmployeeIdentityEnrichmentMiddleware` - but this mapper should still be kept for consistency/defense-in-depth). `KeycloakAdminClient.CreateUserAsync` sets this attribute automatically for newly-provisioned employees — this mapper is what actually puts it on the token. |
| **Hardcoded claim** | Token Claim Name: `tenant_id`, Claim value: `default_tenant`, Add to access token: ON | Finbuckle multi-tenancy (`WithClaimStrategy("tenant_id")`) and `ApplicationDBContext.CurrentTenantId` both read this claim directly. Every other auth path in this backend (Local JWT, DevHeader) hardcodes the same value — Keycloak logins need to match. |

## 3. Confirm realm roles exist

Realm `sylviang` → Realm roles: confirm `Employee`, `Supervisor`, `HR`, `Admin` all exist (exact spelling/case — `RequireRole` is case-sensitive). These are the same names `KeycloakAdminClient.AssignRealmRoleAsync` assigns via the Admin API and `Keycloak:DefaultRealmRole` in `appsettings.json` defaults to.

## 4. Confirm the Admin API service account still works

Prerequisite from the earlier "Grant Access" work: `sylviang-api`'s service account (Clients → `sylviang-api` → Service accounts roles) must have the realm-management **`manage-users`** role (and permission to manage realm role mappings). If `EmployeeCredentialController`'s Grant Access/Reset Password endpoints return a 502 with a permissions message, this is what's missing.

## 5. Existing accounts won't have the `employee_id` attribute yet

The `employee_id` Keycloak user attribute is only set automatically for accounts created **after** the `KeycloakAdminClient.CreateUserAsync` fix that shipped alongside this checklist. The accounts already provisioned before that fix need one of:
- Manually add the `employee_id` attribute (Users → select user → Attributes tab → key `employee_id`, value = their numeric `EmployeeId`) in the Keycloak admin console, or
- Re-provision them (delete the Keycloak user, use "Grant Access" again from the admin UI), or
- Do nothing — `EmployeeIdentityEnrichmentMiddleware` (backend) now resolves `EmployeeId` server-side from `EmployeeKeycloakAccount` via the token's `sub` claim whenever `employee_id` is missing from the token, so this no longer blocks anything functionally. Still worth backfilling for consistency.

Note the **Audience mapper in step 2 has no such backfill concern** — access tokens are short-lived (`expires_in` ~5 minutes per this realm's config), so once the mapper is added, the very next login for any employee (old or new) picks it up automatically.

## Verification

1. Log in through the admin UI's existing form with a real Grant-Access-created employee's username/password — should succeed and land in the app like any other login.
2. Decode the resulting access token (jwt.io or similar) — confirm it carries `aud` including `sylviang-api`, `role` (matching a realm role), `employee_id` (a plain numeric string), and `tenant_id: "default_tenant"`.
3. Call any protected endpoint (e.g. `GET /community/employee/{id}` for the logged-in employee's own record) with that token — should succeed, not 401 with an audience error.
4. Hit an `HRAdminOnly` page/endpoint — should succeed for an HR/Admin-role account, be denied for a plain Employee-role one.
5. Try a wrong password for a real employee, and a nonexistent username — both should show the same generic "Invalid username or password." (confirms no username enumeration leak between the local and Keycloak checks).
