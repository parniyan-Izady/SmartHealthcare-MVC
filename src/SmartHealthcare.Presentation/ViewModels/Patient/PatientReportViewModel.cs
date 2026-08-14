using SmartHealthcare.Application.DTOs;

namespace SmartHealthcare.Presentation.ViewModels.Patient;

public class PatientReportViewModel
{
    public IReadOnlyList<PatientReportDto> Reports { get; set; } = Array.Empty<PatientReportDto>();
    public string? SearchTerm { get; set; }
}
