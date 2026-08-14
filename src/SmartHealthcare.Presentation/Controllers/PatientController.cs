using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Presentation.ViewModels.Patient;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Features.Patients.Queries.GetHighPerformancePatientReport;

namespace SmartHealthcare.Presentation.Controllers;

[Authorize]
public class PatientController : Controller
{
    private readonly ISender _sender;
    private readonly ILogger<PatientController> _logger;

    public PatientController(ISender sender, ILogger<PatientController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, CancellationToken ct)
    {
        var reports = await _sender.Send(new GetHighPerformancePatientReportQuery(), ct);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            reports = reports.Where(r => 
                r.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.NationalCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        var viewModel = new PatientReportViewModel
        {
            Reports = reports,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }
}
