namespace SmartHealthcare.Infrastructure.Exceptions;

public class IdentityException : Exception
{
    public IEnumerable<string> Errors { get; }

    public IdentityException(string message)
        : base(message)
    {
        Errors = Array.Empty<string>();
    }

    public IdentityException(string message, IEnumerable<string> errors)
        : base(message)
    {
        Errors = errors ?? Array.Empty<string>();
    }

    public IdentityException(IEnumerable<string> errors)
        : base(string.Join("; ", errors ?? Array.Empty<string>()))
    {
        Errors = errors ?? Array.Empty<string>();
    }
}
