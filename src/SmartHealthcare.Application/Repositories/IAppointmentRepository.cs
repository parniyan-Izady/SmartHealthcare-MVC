using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Entities;

namespace SmartHealthcare.Application.Repositories;

public interface IAppointmentRepository : IGenericRepository<Appointment>
{
    Task<IReadOnlyList<Appointment>> GetDoctorAppointmentsForDateAsync(Guid doctorId, DateTime dateUtc, CancellationToken cancellationToken = default);
    Task<Appointment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAppointmentsAsync(
        Guid? doctorId,
        Guid? patientId,
        string? status,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken = default);
}
