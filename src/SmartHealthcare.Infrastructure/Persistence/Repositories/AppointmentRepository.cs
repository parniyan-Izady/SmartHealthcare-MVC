using Microsoft.EntityFrameworkCore;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Entities;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Infrastructure.Persistence.DbContext;

namespace SmartHealthcare.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IReadOnlyList<Appointment>> GetDoctorAppointmentsForDateAsync(Guid doctorId, DateTime dateUtc, CancellationToken cancellationToken = default)
    {
        var startOfDay = dateUtc.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbContext.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => a.DoctorId == doctorId &&
                        a.AppointmentStartUtc >= startOfDay &&
                        a.AppointmentStartUtc < endOfDay &&
                        !a.IsDeleted)
            .OrderBy(a => a.AppointmentStartUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Appointment?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);
    }

    public async Task<(IReadOnlyList<Appointment> Items, int TotalCount)> GetPagedAppointmentsAsync(
        Guid? doctorId,
        Guid? patientId,
        string? status,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        int page,
        int pageSize,
        string? sortBy,
        string? sortOrder,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Appointments
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Where(a => !a.IsDeleted);

        if (doctorId.HasValue)
        {
            query = query.Where(a => a.DoctorId == doctorId.Value);
        }

        if (patientId.HasValue)
        {
            query = query.Where(a => a.PatientId == patientId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AppointmentStatus>(status, true, out var parsedStatus))
        {
            query = query.Where(a => a.Status == parsedStatus);
        }

        if (fromDateUtc.HasValue)
        {
            query = query.Where(a => a.AppointmentStartUtc >= fromDateUtc.Value);
        }

        if (toDateUtc.HasValue)
        {
            query = query.Where(a => a.AppointmentEndUtc <= toDateUtc.Value);
        }

        bool isDescending = string.Equals(sortOrder, "desc", StringComparison.OrdinalIgnoreCase);

        query = (sortBy?.ToLower()) switch
        {
            "status" => isDescending ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
            "doctor" => isDescending ? query.OrderByDescending(a => a.Doctor.User.LastName) : query.OrderBy(a => a.Doctor.User.LastName),
            "patient" => isDescending ? query.OrderByDescending(a => a.Patient.User.LastName) : query.OrderBy(a => a.Patient.User.LastName),
            _ => isDescending ? query.OrderByDescending(a => a.AppointmentStartUtc) : query.OrderBy(a => a.AppointmentStartUtc),
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
