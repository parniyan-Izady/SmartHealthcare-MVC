namespace SmartHealthcare.Domain.Exceptions;

public class AppointmentInPastException : DomainException
{
    public AppointmentInPastException()
        : base("Cannot schedule an appointment in the past.") { }

    public AppointmentInPastException(DateTime requestedTimeUtc)
        : base($"Cannot schedule an appointment in the past. Requested time was ({requestedTimeUtc:u}).") { }
}
