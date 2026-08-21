using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdentityUserIdAsync(Guid identityUserId, CancellationToken cancellationToken = default);
}
