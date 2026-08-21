using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PatientReportDto>> GetPatientReportsAsync(CancellationToken cancellationToken = default);
}
