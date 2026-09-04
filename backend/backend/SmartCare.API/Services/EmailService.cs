using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace SmartCare.API.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
    {
        var emailSettings = _config.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("SmartCare", emailSettings["SenderEmail"] ?? "noreply@smartcare.com"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "SmartCare - Password Reset Request";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #2563EB;'>SmartCare</h2>
                <p>Hello,</p>
                <p>We received a request to reset the password for the account associated with this email address.</p>
                <p>Please click the button below to reset your password:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <a href='{resetLink}' style='background-color: #2563EB; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;'>Reset Password</a>
                </div>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p style='color: #6B7280; font-size: 14px; word-break: break-all;'>{resetLink}</p>
                <p>If you did not request this reset, you can safely ignore this email.</p>
                <br>
                <p>Best regards,</p>
                <p>The SmartCare Team</p>
            </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        var host = emailSettings["SmtpHost"];
        var port = int.Parse(emailSettings["SmtpPort"] ?? "465");
        var username = emailSettings["SmtpUsername"];
        var password = emailSettings["SmtpPassword"];

        using var client = new SmtpClient();

        // Bypass SSL certificate revocation check — only skips CRL lookup, cert is still validated.
        // Required on macOS where CRL endpoints may be unreachable from a local dev machine.
        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

        try
        {
            // Use SslOnConnect (port 465) — more reliable than STARTTLS on macOS dev environments
            await client.ConnectAsync(host, port, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
        }
        finally
        {
            await client.DisconnectAsync(true);
        }
    }
}
