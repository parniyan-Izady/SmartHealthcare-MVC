using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Persistence.DbContext;

namespace SmartHealthcare.Infrastructure.Persistence.Repositories;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Patient?> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.NationalCode == nationalCode && !p.IsDeleted, cancellationToken);
    }

    public async Task<Patient?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .FirstOrDefaultAsync(p => p.IdentityUserId == userId && !p.IsDeleted, cancellationToken);
    }

    public async Task<Patient?> GetWithAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);
    }

    public async Task<Patient?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientReportDto>> GetPatientReportsAsync(
        string? searchTerm = null, 
        Guid? doctorId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (doctorId.HasValue)
        {
            query = query.Where(p => p.Appointments.Any(a => a.DoctorId == doctorId.Value && !a.IsDeleted));
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p =>
                p.FirstName.Contains(term) ||
                p.LastName.Contains(term) ||
                p.NationalCode.Contains(term) ||
                p.PhoneNumber.Contains(term));
        }

        return await query
            .OrderByDescending(p => p.Appointments.Count(a => !a.IsDeleted))
            .Select(p => new PatientReportDto(
                p.Id,
                p.FirstName + " " + p.LastName,
                p.NationalCode,
                p.PhoneNumber,
                p.Appointments.Count(a => !a.IsDeleted),
                p.Appointments.Where(a => !a.IsDeleted).Max(a => (DateTime?)a.AppointmentStartUtc)
            ))
            .ToListAsync(cancellationToken);
    }
}
