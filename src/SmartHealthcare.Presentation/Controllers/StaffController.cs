using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Application.Common.Exceptions;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Application.Features.Staff.Commands.CreateStaff;
using SmartHealthcare.Application.Features.Staff.Commands.DeleteStaff;
using SmartHealthcare.Application.Features.Staff.Commands.ResetStaffPassword;
using SmartHealthcare.Application.Features.Staff.Commands.ToggleStaffStatus;
using SmartHealthcare.Application.Features.Staff.Commands.UpdateStaff;
using SmartHealthcare.Application.Features.Staff.Queries.GetStaffMemberById;
using SmartHealthcare.Application.Features.Staff.Queries.GetStaffMembers;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;
using SmartHealthcare.Presentation.ViewModels.Staff;

namespace SmartHealthcare.Presentation.Controllers;

[Authorize(Roles = "SuperAdmin,Admin")]
public class StaffController : Controller
{
    private readonly ISender _sender;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<StaffController> _logger;

    public StaffController(
        ISender sender,
        ICurrentUserService currentUserService,
        ILogger<StaffController> logger)
    {
        _sender = sender;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var staffMembers = await _sender.Send(new GetStaffMembersQuery(), ct);

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

        var viewModel = new StaffListViewModel
        {
            CanCreateAdmin = isSuperAdmin,
            IsSuperAdmin = isSuperAdmin,
            StaffMembers = staffMembers.Select(s => new StaffMemberViewModel
            {
                Id = s.Id,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Role = s.Role,
                IsActive = s.IsActive,
                CreatedAtUtc = s.CreatedAtUtc
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new CreateStaffViewModel
        {
            Role = UserRole.Receptionist,
            AvailableRoles = GetAvailableRoles()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateStaffViewModel model, CancellationToken ct)
    {
        model.AvailableRoles = GetAvailableRoles();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new CreateStaffCommand(
                model.Email,
                model.PhoneNumber,
                model.Password,
                model.Role
            );

            await _sender.Send(command, ct);

            _logger.LogInformation("New staff member {Email} created with role {Role} by {Operator}.",
                model.Email, model.Role, User.Identity?.Name);

            TempData["SuccessMessage"] = $"Staff member ({model.Role}) created successfully!";
            return RedirectToAction(nameof(Index));
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
            ModelState.AddModelError(nameof(model.Role), ex.Message);
            return View(model);
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating staff member.");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the staff account.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        try
        {
            var staff = await _sender.Send(new GetStaffMemberByIdQuery(id), ct);
            var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);

            var model = new EditStaffViewModel
            {
                Id = staff.Id,
                Email = staff.Email,
                PhoneNumber = staff.PhoneNumber ?? string.Empty,
                Role = staff.Role,
                CanChangeRole = isSuperAdmin,
                AvailableRoles = GetAvailableRoles()
            };

            return View(model);
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditStaffViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var isSuperAdmin = _currentUserService.IsInRole(UserRole.SuperAdmin);
        model.CanChangeRole = isSuperAdmin;
        model.AvailableRoles = GetAvailableRoles();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new UpdateStaffCommand(
                model.Id,
                model.Email,
                model.PhoneNumber,
                model.Role
            );

            await _sender.Send(command, ct);

            _logger.LogInformation("Staff member {Email} updated by {Operator}.",
                model.Email, User.Identity?.Name);

            TempData["SuccessMessage"] = $"Staff member {model.Email} updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
            return RedirectToAction(nameof(Index));
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
            _logger.LogError(ex, "Unexpected error updating staff member.");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the staff account.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(Guid id, CancellationToken ct)
    {
        try
        {
            var staff = await _sender.Send(new GetStaffMemberByIdQuery(id), ct);

            var model = new ResetStaffPasswordViewModel
            {
                Id = staff.Id,
                Email = staff.Email
            };

            return View(model);
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
            return RedirectToAction(nameof(Index));
        }
        catch (ForbiddenAccessException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(Guid id, ResetStaffPasswordViewModel model, CancellationToken ct)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _sender.Send(new ResetStaffPasswordCommand(model.Id, model.NewPassword), ct);

            TempData["SuccessMessage"] = $"Password for {model.Email} has been reset successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
            return RedirectToAction(nameof(Index));
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
            _logger.LogError(ex, "Unexpected error resetting staff password.");
            ModelState.AddModelError(string.Empty, "An error occurred while resetting the password.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id, CancellationToken ct)
    {
        try
        {
            var newStatus = await _sender.Send(new ToggleStaffStatusCommand(id), ct);
            var statusText = newStatus ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"User status has been {statusText} successfully.";
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
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
            _logger.LogError(ex, "Unexpected error toggling staff status.");
            TempData["ErrorMessage"] = "An error occurred while changing user status.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _sender.Send(new DeleteStaffCommand(id), ct);
            TempData["SuccessMessage"] = "Staff member has been deleted successfully.";
        }
        catch (NotFoundException)
        {
            TempData["ErrorMessage"] = "Staff member not found.";
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
            _logger.LogError(ex, "Unexpected error deleting staff member.");
            TempData["ErrorMessage"] = "An error occurred while deleting staff member.";
        }

        return RedirectToAction(nameof(Index));
    }

    private List<UserRole> GetAvailableRoles() =>
        _currentUserService.IsInRole(UserRole.SuperAdmin)
            ? new List<UserRole> { UserRole.Receptionist, UserRole.Admin }
            : new List<UserRole> { UserRole.Receptionist };
}
