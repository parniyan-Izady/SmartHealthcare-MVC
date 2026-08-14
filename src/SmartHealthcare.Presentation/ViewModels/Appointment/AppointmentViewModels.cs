using System.ComponentModel.DataAnnotations;
using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Presentation.ViewModels.Appointment;

public class AppointmentListViewModel
{
    public IReadOnlyList<AppointmentResponse> Appointments { get; set; } = Array.Empty<AppointmentResponse>();
    public Guid? DoctorId { get; set; }
    public Guid? PatientId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDateUtc { get; set; }
    public DateTime? ToDateUtc { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
    public string? SortBy { get; set; } = "StartUtc";
    public string? SortOrder { get; set; } = "asc";
}

public class AppointmentBookViewModel
{
    [Required(ErrorMessage = "Patient ID is required.")]
    [Display(Name = "Patient ID")]
    public Guid PatientId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required.")]
    [Display(Name = "Doctor ID")]
    public Guid DoctorId { get; set; }

    [Required(ErrorMessage = "Start time is required.")]
    [Display(Name = "Start Time (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime StartTimeUtc { get; set; } = DateTime.UtcNow.Date.AddHours(9);

    [Required(ErrorMessage = "End time is required.")]
    [Display(Name = "End Time (UTC)")]
    [DataType(DataType.DateTime)]
    public DateTime EndTimeUtc { get; set; } = DateTime.UtcNow.Date.AddHours(10);

    [Display(Name = "Reason for Visit")]
    [StringLength(500)]
    public string? ReasonForVisit { get; set; }

    // Dropdown helpers
    public IReadOnlyList<DoctorResponse> AvailableDoctors { get; set; } = Array.Empty<DoctorResponse>();
}

public class AppointmentCancelViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Cancellation reason is required.")]
    [StringLength(500)]
    [Display(Name = "Cancellation Reason")]
    public string Reason { get; set; } = string.Empty;
}

public class AppointmentDetailsViewModel
{
    public AppointmentResponse Appointment { get; set; } = null!;
}
