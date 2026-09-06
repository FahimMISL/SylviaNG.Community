namespace SylviaNG.Community.Application.Interfaces.Externals
{
    /// <summary>
    /// Sends transactional emails via SMTP (see SmtpEmailService). Purpose-built for the one
    /// use case that currently needs it (Grant Access welcome email) rather than a generic
    /// message-sending abstraction - add methods here only as new concrete needs arise.
    /// </summary>
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string? toName, string username, string temporaryPassword, CancellationToken cancellationToken = default);
    }
}
