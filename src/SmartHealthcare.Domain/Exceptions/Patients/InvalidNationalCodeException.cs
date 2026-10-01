namespace SmartHealthcare.Domain.Exceptions;

public class InvalidNationalCodeException : DomainException
{
    public InvalidNationalCodeException()
        : base("National code is invalid. It must be a valid 10-digit number.") { }

    public InvalidNationalCodeException(string nationalCode)
        : base($"National code '{nationalCode}' is invalid.") { }
}
