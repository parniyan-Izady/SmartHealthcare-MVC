using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
{
    Task<IReadOnlyList<MedicalRecord>> GetPatientMedicalRecordsAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<MedicalRecord?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
}
