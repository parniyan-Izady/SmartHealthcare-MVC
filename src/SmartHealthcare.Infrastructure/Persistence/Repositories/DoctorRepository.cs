using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Persistence.DbContext;

namespace SmartHealthcare.Infrastructure.Persistence.Repositories;

public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
{
    public DoctorRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Doctor?> GetByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.MedicalLicenseNumber == licenseNumber && !d.IsDeleted, cancellationToken);
    }

    public async Task<Doctor?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Include(d => d.User)
            .FirstOrDefaultAsync(d => d.UserId == userId && !d.IsDeleted, cancellationToken);
    }

    public async Task<Doctor?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Include(d => d.User)
            .Include(d => d.Appointments)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<Doctor>> SearchBySpecialtyAsync(string specialty, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Doctors
            .Include(d => d.User)
            .Where(d => d.MedicalSpecialty.ToLower().Contains(specialty.ToLower()) && !d.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Doctor> Items, int TotalCount)> GetPagedDoctorsAsync(
        string? specialty,
        string? searchTerm,
        bool? isActive,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Doctors
            .Include(d => d.User)
            .Where(d => !d.IsDeleted);

        if (!string.IsNullOrWhiteSpace(specialty))
        {
            query = query.Where(d => d.MedicalSpecialty.ToLower().Contains(specialty.ToLower()));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(d =>
                d.User.FirstName.ToLower().Contains(term) ||
                d.User.LastName.ToLower().Contains(term) ||
                d.MedicalSpecialty.ToLower().Contains(term) ||
                d.MedicalLicenseNumber.ToLower().Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(d => d.User.IsActive == isActive.Value);
        }

        bool isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sortBy?.ToLower()) switch
        {
            "firstname" => isDescending ? query.OrderByDescending(d => d.User.FirstName) : query.OrderBy(d => d.User.FirstName),
            "specialty" => isDescending ? query.OrderByDescending(d => d.MedicalSpecialty) : query.OrderBy(d => d.MedicalSpecialty),
            "consultationfee" => isDescending ? query.OrderByDescending(d => d.ConsultationFee) : query.OrderBy(d => d.ConsultationFee),
            _ => isDescending ? query.OrderByDescending(d => d.User.LastName) : query.OrderBy(d => d.User.LastName),
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
