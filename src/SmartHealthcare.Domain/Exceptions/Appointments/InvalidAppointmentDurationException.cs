namespace SmartHealthcare.Domain.Exceptions;

public class InvalidAppointmentDurationException : DomainException
{
    public InvalidAppointmentDurationException()
        : base("Appointment end time must be after the start time.") { }

    public InvalidAppointmentDurationException(DateTime startUtc, DateTime endUtc)
        : base($"Invalid appointment duration: start time ({startUtc:u}) must be earlier than end time ({endUtc:u}).") { }
}
