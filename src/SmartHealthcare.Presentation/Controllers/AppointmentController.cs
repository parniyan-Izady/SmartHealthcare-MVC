using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Presentation.ViewModels.Appointment;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Features.Appointments.Commands.BookAppointment;
using SmartHealthcare.Application.Features.Appointments.Commands.CancelAppointment;
using SmartHealthcare.Application.Features.Appointments.Commands.CompleteAppointment;
using SmartHealthcare.Application.Features.Appointments.Queries.GetAppointmentById;
using SmartHealthcare.Application.Features.Appointments.Queries.GetPagedAppointments;
using SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;
using SmartHealthcare.Application.Repositories;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Presentation.Controllers;

[Authorize]
public class AppointmentController : Controller
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<AppointmentController> _logger;

    public AppointmentController(
        ISender sender,
        ICurrentUserService currentUserService,
        ILogger<AppointmentController> logger)
    {
        _sender = sender;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        Guid? doctorId,
        Guid? patientId,
        string? status,
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        int page = 1,
        int pageSize = 10,
        string? sortBy = "StartUtc",
        string? sortOrder = "asc",
        CancellationToken ct = default)
    {
        var isStaff = _currentUserService.IsInRole(UserRole.Admin, UserRole.Receptionist);

        if (!isStaff)
        {
            if (_currentUserService.IsInRole(UserRole.Doctor))
            {
                doctorId = _currentUserService.DoctorId;
            }
            else if (_currentUserService.IsInRole(UserRole.Patient))
            {
                patientId = _currentUserService.PatientId;
            }
        }

        var query = new GetPagedAppointmentsQuery(
            doctorId,
            patientId,
            status,
            fromDateUtc,
            toDateUtc,
            page,
            pageSize,
            sortBy,
            sortOrder
        );

        var result = await _sender.Send(query, ct);

        var viewModel = new AppointmentListViewModel
        {
            Appointments = result.Items,
            DoctorId = doctorId,
            PatientId = patientId,
            Status = status,
            FromDateUtc = fromDateUtc,
            ToDateUtc = toDateUtc,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            SortBy = sortBy,
            SortOrder = sortOrder
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        try
        {
            var appointment = await _sender.Send(new GetAppointmentByIdQuery(id), ct);
            if (appointment is null)
            {
                TempData["ErrorMessage"] = $"Appointment with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(new AppointmentDetailsViewModel { Appointment = appointment });
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Patient,Receptionist")]
    public async Task<IActionResult> Book(CancellationToken ct)
    {
        var doctorsResult = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);

        var model = new AppointmentBookViewModel
        {
            AvailableDoctors = doctorsResult.Items,
            StartTimeUtc = DateTime.UtcNow.Date.AddDays(1).AddHours(9),
            EndTimeUtc = DateTime.UtcNow.Date.AddDays(1).AddHours(10)
        };

        if (_currentUserService.IsInRole(UserRole.Patient) && _currentUserService.PatientId.HasValue)
        {
            model.PatientId = _currentUserService.PatientId.Value;
        }

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Patient,Receptionist")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(AppointmentBookViewModel model, CancellationToken ct)
    {
        try
        {
            if (_currentUserService.IsInRole(UserRole.Patient))
            {
                if (!_currentUserService.PatientId.HasValue)
                {
                    ModelState.AddModelError(string.Empty, "Patient profile not found for the current account.");
                    var doctors = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);
                    model.AvailableDoctors = doctors.Items;
                    return View(model);
                }
                model.PatientId = _currentUserService.PatientId.Value;
            }

            var command = new BookAppointmentCommand(
                model.PatientId,
                model.DoctorId,
                model.StartTimeUtc,
                model.EndTimeUtc,
                model.ReasonForVisit
            );

            var appointment = await _sender.Send(command, ct);
            TempData["SuccessMessage"] = "Appointment booked successfully!";
            return RedirectToAction(nameof(Details), new { id = appointment.Id });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            var doctorsResult = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);
            model.AvailableDoctors = doctorsResult.Items;
            return View(model);
        }
        catch (ForbiddenAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var doctorsResult = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);
            model.AvailableDoctors = doctorsResult.Items;
            return View(model);
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            var doctorsResult = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);
            model.AvailableDoctors = doctorsResult.Items;
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment.");
            ModelState.AddModelError(string.Empty, "An error occurred while booking the appointment.");
            var doctorsResult = await _sender.Send(new GetPagedDoctorsQuery(PageSize: 100, IsActive: true), ct);
            model.AvailableDoctors = doctorsResult.Items;
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor,Patient,Receptionist")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string reason, CancellationToken ct)
    {
        try
        {
            var command = new CancelAppointmentCommand(id, reason);
            bool cancelled = await _sender.Send(command, ct);
            if (!cancelled)
            {
                TempData["ErrorMessage"] = $"Appointment with ID '{id}' was not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "Appointment cancelled successfully.";
            }
        }
        catch (ValidationException ex)
        {
            TempData["ErrorMessage"] = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (DomainException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling appointment {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the appointment.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        try
        {
            var command = new CompleteAppointmentCommand(id);
            bool completed = await _sender.Send(command, ct);
            if (!completed)
            {
                TempData["ErrorMessage"] = $"Appointment with ID '{id}' was not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "Appointment marked as completed.";
            }
        }
        catch (ValidationException ex)
        {
            TempData["ErrorMessage"] = string.Join("; ", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (DomainException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing appointment {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while completing the appointment.";
        }

        return RedirectToAction(nameof(Details), new { id });
    }
}
