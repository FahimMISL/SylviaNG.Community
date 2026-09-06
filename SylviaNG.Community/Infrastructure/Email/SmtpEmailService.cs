using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SylviaNG.Community.Application.Interfaces.Externals;

namespace SylviaNG.Community.Infrastructure.Email
{
    /// <summary>
    /// Sends transactional emails via SMTP (MailKit). Configuration lives under the "Smtp"
    /// section of appsettings.json, following the same direct-IConfiguration-read convention
    /// used elsewhere in this codebase (see KeycloakAdminClient) rather than a strongly-typed
    /// options class.
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string? toName, string username, string temporaryPassword, CancellationToken cancellationToken = default)
        {
            var host = _configuration["Smtp:Host"]
                ?? throw new InvalidOperationException("Smtp:Host is not configured.");
            var port = int.TryParse(_configuration["Smtp:Port"], out var parsedPort) ? parsedPort : 587;
            var enableSsl = !bool.TryParse(_configuration["Smtp:EnableSsl"], out var parsedSsl) || parsedSsl;
            var smtpUsername = _configuration["Smtp:Username"];
            var smtpPassword = _configuration["Smtp:Password"];
            var fromAddress = _configuration["Smtp:FromAddress"]
                ?? throw new InvalidOperationException("Smtp:FromAddress is not configured.");
            var fromName = _configuration["Smtp:FromName"] ?? "SylviaNG Community";

            var displayName = toName ?? username;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromAddress));
            message.To.Add(new MailboxAddress(displayName, toEmail));
            message.Subject = "Your SylviaNG Community account is ready";

            // A multipart/alternative body (text + html) with a softer, standard
            // account-welcome tone reads much less like credential-phishing spam to mail
            // filters than a bare plaintext "Username:/Password:" dump did previously.
            var builder = new BodyBuilder
            {
                TextBody =
                    $"Hi {displayName},\n\n" +
                    "Welcome to SylviaNG Community! Your account has been set up and is ready to use.\n\n" +
                    $"Here are your login details:\n" +
                    $"Username: {username}\n" +
                    $"Password: {temporaryPassword}\n\n" +
                    "For your security, please sign in and change this password soon, and avoid sharing it with anyone.\n\n" +
                    "This is an automated message from SylviaNG Community. If you weren't expecting this, " +
                    "please contact your HR or IT administrator.\n\n" +
                    "- SylviaNG Community",
                HtmlBody =
                    $"<p>Hi {System.Net.WebUtility.HtmlEncode(displayName)},</p>" +
                    "<p>Welcome to SylviaNG Community! Your account has been set up and is ready to use.</p>" +
                    "<p>Here are your login details:</p>" +
                    "<p>" +
                    $"Username: <strong>{System.Net.WebUtility.HtmlEncode(username)}</strong><br>" +
                    $"Password: <strong>{System.Net.WebUtility.HtmlEncode(temporaryPassword)}</strong>" +
                    "</p>" +
                    "<p>For your security, please sign in and change this password soon, and avoid sharing it with anyone.</p>" +
                    "<p style=\"color:#666;font-size:12px;\">This is an automated message from SylviaNG Community. " +
                    "If you weren't expecting this, please contact your HR or IT administrator.</p>" +
                    "<p>- SylviaNG Community</p>"
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, enableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None, cancellationToken);

            if (!string.IsNullOrEmpty(smtpUsername))
                await client.AuthenticateAsync(smtpUsername, smtpPassword, cancellationToken);

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Welcome email sent to {ToEmail} for username {Username}.", toEmail, username);
        }
    }
}
