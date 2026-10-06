using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Presentation.ViewModels.Account;
using SmartHealthcare.Application.Features.Auth.Commands.RegisterUser;
using SmartHealthcare.Domain.Enums;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly ISender _sender;
    private readonly ISignInService _signInService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ISender sender,
        ISignInService signInService,
        ILogger<AccountController> logger)
    {
        _sender = sender;
        _signInService = signInService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var (succeeded, isLockedOut, requiresTwoFactor, errorMessage) = await _signInService.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (isLockedOut)
            {
                _logger.LogWarning("User account locked out for {Email}.", model.Email);
                return View("Lockout");
            }

            if (!succeeded)
            {
                ModelState.AddModelError(string.Empty, errorMessage ?? "Invalid email or password.");
                return View(model);
            }

            _logger.LogInformation("User {Email} logged in successfully.", model.Email);
            TempData["SuccessMessage"] = "Welcome back!";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login.");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during login. Please try again.");
            return View(model);
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new RegisterUserCommand(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password,
                UserRole.Patient
            );

            var authResponse = await _sender.Send(command, ct);

            await _signInService.SignInUserAsync(authResponse.UserId, isPersistent: false);

            _logger.LogInformation("New user {Email} registered successfully.", model.Email);
            TempData["SuccessMessage"] = "Registration completed successfully! Welcome aboard.";
            return RedirectToAction("Index", "Home");
        }
        catch (ValidationException ex)
        {
            foreach (var error in ex.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return View(model);
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration.");
            ModelState.AddModelError(string.Empty, "An unexpected error occurred during registration. Please try again.");
            return View(model);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInService.SignOutAsync();
        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
