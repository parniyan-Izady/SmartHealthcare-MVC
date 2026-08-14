using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Domain.Entities;

public class Appointment : BaseEntity
{
    public Guid PatientId { get; private set; }
    public Patient Patient { get; private set; } = default!;
    
    public Guid DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;

    public DateTime AppointmentStartUtc { get; private set; }
    public DateTime AppointmentEndUtc { get; private set; }
    public AppointmentStatus Status { get; private set; } = AppointmentStatus.Scheduled;
    public string? ReasonForVisit { get; private set; }
    public string? CancellationReason { get; private set; }

    private Appointment() { }

    public Appointment(Guid patientId, Guid doctorId, DateTime startUtc, DateTime endUtc, string? reason)
    {
        if (endUtc <= startUtc)
        {
            throw new ArgumentException("Appointment end time must be after start time.");
        }

        PatientId = patientId;
        DoctorId = doctorId;
        AppointmentStartUtc = startUtc;
        AppointmentEndUtc = endUtc;
        ReasonForVisit = reason;
        Status = AppointmentStatus.Scheduled;
    }

    public void Confirm()
    {
        Status = AppointmentStatus.Confirmed;
        MarkUpdated();
    }

    public void Cancel(string reason)
    {
        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        MarkUpdated();
    }

    public void Complete()
    {
        Status = AppointmentStatus.Completed;
        MarkUpdated();
    }
}
