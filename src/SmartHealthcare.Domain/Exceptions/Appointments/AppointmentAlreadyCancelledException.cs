namespace SmartHealthcare.Domain.Exceptions;

public class AppointmentAlreadyCancelledException : DomainException
{
    public AppointmentAlreadyCancelledException()
        : base("Appointment is already cancelled.") { }

    public AppointmentAlreadyCancelledException(Guid appointmentId)
        : base($"Appointment with ID ({appointmentId}) is already cancelled.") { }
}
