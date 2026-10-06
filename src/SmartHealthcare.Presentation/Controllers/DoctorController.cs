using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Presentation.ViewModels.Doctor;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Features.Doctors.Commands.CreateDoctor;
using SmartHealthcare.Application.Features.Doctors.Commands.DeleteDoctor;
using SmartHealthcare.Application.Features.Doctors.Commands.UpdateDoctor;
using SmartHealthcare.Application.Features.Doctors.Queries.GetDoctorById;
using SmartHealthcare.Application.Features.Doctors.Queries.GetPagedDoctors;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Presentation.Controllers;

public class DoctorController : Controller
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DoctorController> _logger;

    public DoctorController(
        ISender sender,
        ICurrentUserService currentUserService,
        ILogger<DoctorController> logger)
    {
        _sender = sender;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? specialty,
        string? searchTerm,
        bool? isActive,
        int page = 1,
        int pageSize = 10,
        string? sortBy = "LastName",
        string? sortOrder = "asc",
        CancellationToken ct = default)
    {
        var query = new GetPagedDoctorsQuery(
            specialty,
            searchTerm,
            isActive,
            page,
            pageSize,
            sortBy,
            sortOrder
        );

        var result = await _sender.Send(query, ct);

        var viewModel = new DoctorListViewModel
        {
            Doctors = result.Items,
            Specialty = specialty,
            SearchTerm = searchTerm,
            IsActive = isActive,
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
        var doctor = await _sender.Send(new GetDoctorByIdQuery(id), ct);
        if (doctor is null)
        {
            TempData["ErrorMessage"] = $"Doctor with ID '{id}' was not found.";
            return RedirectToAction(nameof(Index));
        }

        return View(new DoctorDetailsViewModel { Doctor = doctor });
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View(new DoctorCreateViewModel());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DoctorCreateViewModel model, CancellationToken ct)
    {
        try
        {
            var command = new CreateDoctorCommand(
                model.FirstName,
                model.LastName,
                model.Email,
                model.PhoneNumber,
                model.Password,
                model.MedicalLicenseNumber,
                model.MedicalSpecialty,
                model.ConsultationFee,
                model.OfficeAddress
            );

            var doctor = await _sender.Send(command, ct);
            TempData["SuccessMessage"] = $"Dr. {doctor.FullName} was created successfully!";
            return RedirectToAction(nameof(Details), new { id = doctor.Id });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor.");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the doctor.");
            return View(model);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Doctor")]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var doctor = await _sender.Send(new GetDoctorByIdQuery(id), ct);
        if (doctor is null)
        {
            TempData["ErrorMessage"] = $"Doctor with ID '{id}' was not found.";
            return RedirectToAction(nameof(Index));
        }

        if (!_currentUserService.IsInRole(UserRole.Admin) && doctor.Id != _currentUserService.DoctorId)
        {
            TempData["ErrorMessage"] = "You do not have permission to edit another doctor's profile.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var model = new DoctorEditViewModel
        {
            Id = doctor.Id,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            MedicalLicenseNumber = doctor.MedicalLicenseNumber,
            MedicalSpecialty = doctor.MedicalSpecialty,
            ConsultationFee = doctor.ConsultationFee,
            OfficeAddress = doctor.OfficeAddress
        };

        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Doctor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, DoctorEditViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        try
        {
            var command = new UpdateDoctorCommand(
                id,
                model.FirstName,
                model.LastName,
                model.MedicalSpecialty,
                model.ConsultationFee,
                model.OfficeAddress
            );

            var updated = await _sender.Send(command, ct);
            if (updated is null)
            {
                TempData["ErrorMessage"] = $"Doctor with ID '{id}' was not found.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Dr. {updated.FullName} was updated successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating doctor.");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the doctor.");
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            bool deleted = await _sender.Send(new DeleteDoctorCommand(id), ct);
            if (!deleted)
            {
                TempData["ErrorMessage"] = $"Doctor with ID '{id}' was not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "Doctor was deleted successfully.";
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {Id}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the doctor.";
        }

        return RedirectToAction(nameof(Index));
    }
}
