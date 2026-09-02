using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace SmartCare.API.Services;

public sealed class EmailOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; } = false;
    public string FromName { get; set; } = "SmartCare";
    public string FromEmail { get; set; } = "no-reply@smartcare.local";
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(toEmail))
        {
            throw new ArgumentException("Recipient email is required.", nameof(toEmail));
        }

        if (string.IsNullOrWhiteSpace(resetLink))
        {
            throw new ArgumentException("Reset link is required.", nameof(resetLink));
        }

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = !string.IsNullOrWhiteSpace(_options.Username)
                ? new NetworkCredential(_options.Username, _options.Password)
                : CredentialCache.DefaultNetworkCredentials,
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        var subject = "SmartCare Password Reset";
        var body = $"""
            <html>
              <body>
                <h2>Password reset request</h2>
                <p>We received a request to reset your password for SmartCare.</p>
                <p>Use the secure link below to continue:</p>
                <p><a href=""{resetLink}"">{resetLink}</a></p>
                <p>If you did not request this, you can ignore this email.</p>
              </body>
            </html>
            """;

        using var message = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
            BodyEncoding = System.Text.Encoding.UTF8
        };

        message.To.Add(toEmail);

        try
        {
            await client.SendMailAsync(message, cancellationToken);
            _logger.LogInformation("Password reset email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
            throw;
        }
    }
}
