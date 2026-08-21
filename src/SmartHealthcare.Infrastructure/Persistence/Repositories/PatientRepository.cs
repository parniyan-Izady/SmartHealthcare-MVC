using Microsoft.EntityFrameworkCore;
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
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.NationalCode == nationalCode && !p.IsDeleted, cancellationToken);
    }

    public async Task<Patient?> GetWithAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Include(p => p.User)
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == patientId && !p.IsDeleted, cancellationToken);
    }

    public async Task<Patient?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Include(p => p.User)
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<PatientReportDto>> GetPatientReportsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .Select(p => new PatientReportDto(
                p.Id,
                p.User != null ? p.User.FirstName + " " + p.User.LastName : string.Empty,
                p.NationalCode,
                p.PhoneNumber,
                p.Appointments.Count(a => !a.IsDeleted),
                p.Appointments.Where(a => !a.IsDeleted).Max(a => (DateTime?)a.AppointmentStartUtc)
            ))
            .OrderByDescending(r => r.TotalAppointmentsCount)
            .ToListAsync(cancellationToken);
    }
}
