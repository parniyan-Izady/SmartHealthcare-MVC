using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Infrastructure.Services;

public class EmailSender : IEmailSender
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IOptions<EmailSettings> emailOptions, ILogger<EmailSender> logger)
    {
        _emailSettings = emailOptions.Value;
        _logger = logger;
    }

    public Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default)
    {
        return SendEmailAsync(message.Recipient, message.Subject, message.Content, cancellationToken);
    }

    public async Task SendEmailAsync(string recipient, string subject, string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_emailSettings.Host))
        {
            _logger.LogWarning("EmailSettings:Host is not configured. Emulating email dispatch to {Recipient}: {Subject}", recipient, subject);
            _logger.LogInformation("Email Body:\n{Content}", content);
            return;
        }

        using var client = new SmtpClient(_emailSettings.Host, _emailSettings.Port)
        {
            EnableSsl = _emailSettings.EnableSsl,
            Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.Password)
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
            Subject = subject,
            Body = content,
            IsBodyHtml = true
        };

        mailMessage.To.Add(recipient);

        try
        {
            _logger.LogInformation("Sending email to {Recipient} via SMTP...", recipient);
            await client.SendMailAsync(mailMessage, cancellationToken);
            _logger.LogInformation("Email successfully sent to {Recipient}.", recipient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}.", recipient);
            throw;
        }
    }
}