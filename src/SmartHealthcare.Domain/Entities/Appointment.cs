using SmartHealthcare.Domain.Common;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

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
            throw new InvalidAppointmentDurationException(startUtc, endUtc);
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
        if (Status == AppointmentStatus.Cancelled)
        {
            throw new AppointmentAlreadyCancelledException(Id);
        }

        if (Status == AppointmentStatus.Completed)
        {
            throw new AppointmentAlreadyCompletedException(Id);
        }

        Status = AppointmentStatus.Confirmed;
        MarkUpdated();
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new AppointmentCancellationReasonRequiredException();
        }

        if (Status == AppointmentStatus.Cancelled)
        {
            throw new AppointmentAlreadyCancelledException(Id);
        }

        if (Status == AppointmentStatus.Completed)
        {
            throw new AppointmentAlreadyCompletedException(Id);
        }

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason;
        MarkUpdated();
    }

    public void Complete()
    {
        if (Status == AppointmentStatus.Cancelled)
        {
            throw new AppointmentAlreadyCancelledException(Id);
        }

        Status = AppointmentStatus.Completed;
        MarkUpdated();
    }
}
