using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? DoctorId { get; }
    Guid? PatientId { get; }
    UserRole? Role { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(params UserRole[] roles);
}
