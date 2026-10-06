using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface INotificationSender
{
    Task SendAsync(NotificationMessage message, CancellationToken cancellationToken = default);
}
