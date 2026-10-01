namespace SmartHealthcare.Domain.Exceptions;

public class AppointmentAlreadyCompletedException : DomainException
{
    public AppointmentAlreadyCompletedException()
        : base("Cannot modify or cancel an appointment that has already been completed.") { }

    public AppointmentAlreadyCompletedException(Guid appointmentId)
        : base($"Appointment with ID ({appointmentId}) has already been completed and cannot be modified.") { }
}
