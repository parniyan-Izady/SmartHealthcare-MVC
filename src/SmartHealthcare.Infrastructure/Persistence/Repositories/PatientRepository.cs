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
}
