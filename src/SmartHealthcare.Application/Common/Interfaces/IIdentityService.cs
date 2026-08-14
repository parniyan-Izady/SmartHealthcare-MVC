namespace SmartHealthcare.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, Guid IdentityUserId, IEnumerable<string> Errors)> CreateIdentityUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Succeeded, Guid IdentityUserId, IEnumerable<string> Errors)> ValidateUserCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<bool> DeleteIdentityUserAsync(Guid identityUserId, CancellationToken cancellationToken = default);
}
