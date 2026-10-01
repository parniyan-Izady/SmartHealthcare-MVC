namespace SmartHealthcare.Domain.Exceptions;

public class InvalidConsultationFeeException : DomainException
{
    public InvalidConsultationFeeException()
        : base("Consultation fee cannot be negative.") { }

    public InvalidConsultationFeeException(decimal fee)
        : base($"Consultation fee ({fee:C}) is invalid. It cannot be a negative amount.") { }
}
