using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IDoctorRepository : IGenericRepository<Doctor>
{
    Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Doctor?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Doctor>> SearchBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Doctor> Items, int TotalCount)> GetPagedDoctorsAsync(
        string? specialty,
        string? searchTerm,
        bool? isActive,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken = default);
}
