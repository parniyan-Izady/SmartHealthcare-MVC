namespace SmartHealthcare.Domain.Exceptions;

public class DuplicateMedicalLicenseException : DomainException
{
    public DuplicateMedicalLicenseException(string licenseNumber)
        : base($"A doctor with medical license number '{licenseNumber}' already exists.") { }
}
