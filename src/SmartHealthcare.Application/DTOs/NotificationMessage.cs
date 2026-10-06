namespace SmartHealthcare.Application.DTOs;

public record NotificationMessage(
    string Recipient,
    string Subject,
    string Content
);
