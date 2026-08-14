using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Infrastructure.Persistence.DbContext;

namespace SmartHealthcare.Infrastructure.Persistence.Repositories;

public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<MedicalRecord>> GetPatientMedicalRecordsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MedicalRecords
            .Include(m => m.Patient).ThenInclude(p => p.User)
            .Include(m => m.Doctor).ThenInclude(d => d.User)
            .Where(m => m.PatientId == patientId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<MedicalRecord?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MedicalRecords
            .Include(m => m.Patient).ThenInclude(p => p.User)
            .Include(m => m.Doctor).ThenInclude(d => d.User)
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);
    }
}
