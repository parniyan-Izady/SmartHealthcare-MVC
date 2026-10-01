namespace SmartHealthcare.Domain.Exceptions;

public class InvalidDateOfBirthException : DomainException
{
    public InvalidDateOfBirthException()
        : base("Date of birth cannot be in the future.") { }

    public InvalidDateOfBirthException(DateTime dateOfBirth)
        : base($"Date of birth ({dateOfBirth:yyyy-MM-dd}) is invalid. It cannot be in the future.") { }
}
