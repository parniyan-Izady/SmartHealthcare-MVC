using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface IEmailSender : INotificationSender
{
    Task SendEmailAsync(string recipient, string subject, string content, CancellationToken cancellationToken = default);
}
