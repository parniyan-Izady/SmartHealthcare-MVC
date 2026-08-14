using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IPatientRepository : IGenericRepository<Patient>
{
    Task<Patient?> GetByNationalCodeAsync(string nationalCode, CancellationToken cancellationToken = default);
    Task<Patient?> GetWithAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default);
}
