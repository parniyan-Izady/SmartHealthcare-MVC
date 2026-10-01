namespace SmartHealthcare.Domain.Exceptions;

public class DoctorScheduleConflictException : DomainException
{
    public DoctorScheduleConflictException()
        : base("The doctor already has another appointment scheduled during this time slot.") { }

    public DoctorScheduleConflictException(Guid doctorId, DateTime startUtc, DateTime endUtc)
        : base($"Doctor ({doctorId}) is not available between ({startUtc:u}) and ({endUtc:u}).") { }
}
