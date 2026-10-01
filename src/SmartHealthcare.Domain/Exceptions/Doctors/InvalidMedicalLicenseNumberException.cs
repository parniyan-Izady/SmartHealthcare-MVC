namespace SmartHealthcare.Domain.Exceptions;

public class InvalidMedicalLicenseNumberException : DomainException
{
    public InvalidMedicalLicenseNumberException()
        : base("Medical license number is invalid or cannot be empty.") { }

    public InvalidMedicalLicenseNumberException(string licenseNumber)
        : base($"Medical license number '{licenseNumber}' is invalid.") { }
}
