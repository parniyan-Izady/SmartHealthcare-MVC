using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Presentation.ViewModels;
using SmartHealthcare.Presentation.ViewModels.Home;
using SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;
using SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;
using SmartHealthcare.Application.Features.Patients.Queries.GetPatientsCount;

namespace SmartHealthcare.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly ISender _sender;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ISender sender, ILogger<HomeController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var doctors = await _sender.Send(new GetPagedDoctorsQuery(Page: 1, PageSize: 5, SortBy: "LastName", SortOrder: "asc"), ct);
        var appointments = await _sender.Send(new GetPagedAppointmentsQuery(Page: 1, PageSize: 5, SortBy: "StartUtc", SortOrder: "asc"), ct);
        var totalPatients = await _sender.Send(new GetPatientsCountQuery(), ct);

        var viewModel = new DashboardViewModel
        {
            TotalDoctors = doctors.TotalCount,
            TotalAppointments = appointments.TotalCount,
            TotalPatients = totalPatients,
            RecentDoctors = doctors.Items,
            UpcomingAppointments = appointments.Items
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
