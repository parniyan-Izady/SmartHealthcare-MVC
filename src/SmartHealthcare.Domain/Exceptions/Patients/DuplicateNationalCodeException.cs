namespace SmartHealthcare.Domain.Exceptions;

public class DuplicateNationalCodeException : DomainException
{
    public DuplicateNationalCodeException(string nationalCode)
        : base($"A patient with national code '{nationalCode}' is already registered.") { }
}
