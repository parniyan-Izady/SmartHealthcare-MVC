namespace SmartHealthcare.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("You do not have permission to access this resource or perform this action.") { }

    public ForbiddenAccessException(string message)
        : base(message) { }
}
