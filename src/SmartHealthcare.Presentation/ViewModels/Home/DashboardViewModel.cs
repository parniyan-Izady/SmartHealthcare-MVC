using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Presentation.ViewModels.Home;

public class DashboardViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalAppointments { get; set; }
    public int TotalPatients { get; set; }
    public IReadOnlyList<DoctorResponse> RecentDoctors { get; set; } = Array.Empty<DoctorResponse>();
    public IReadOnlyList<AppointmentResponse> UpcomingAppointments { get; set; } = Array.Empty<AppointmentResponse>();
}
