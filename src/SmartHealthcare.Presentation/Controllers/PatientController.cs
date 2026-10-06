using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.DTOs;
using SmartHealthcare.Application.Features.Patients.Queries.GetPatientReports;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Presentation.ViewModels.Patient;

namespace SmartHealthcare.Presentation.Controllers;

[Authorize(Roles = "Admin,Doctor,Nurse,Receptionist")]
public class PatientController : Controller
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<PatientController> _logger;

    public PatientController(
        ISender sender, 
        ICurrentUserService currentUserService,
        ILogger<PatientController> logger)
    {
        _sender = sender;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, CancellationToken ct)
    {
        Guid? doctorId = _currentUserService.IsInRole(UserRole.Doctor) ? _currentUserService.DoctorId : null;

        var reports = await _sender.Send(new GetPatientReportsQuery(searchTerm, doctorId), ct);

        var viewModel = new PatientReportViewModel
        {
            Reports = reports,
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }
}
