namespace SmartHealthcare.Domain.Exceptions;

public class AppointmentCancellationReasonRequiredException : DomainException
{
    public AppointmentCancellationReasonRequiredException()
        : base("A valid cancellation reason must be provided when cancelling an appointment.") { }
}
