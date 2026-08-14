using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartHealthcare.Presentation.ViewModels.Account;
using SmartHealthcare.Application.Features.Auth.Commands.LoginUser;
using SmartHealthcare.Application.Features.Auth.Commands.RegisterUser;
using SmartHealthcare.Domain.Exceptions;

namespace SmartHealthcare.Presentation.Controllers;

public class AccountController : Controller
{
    private readonly ISender _sender;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ISender sender, ILogger<AccountController> logger)
    {
        _sender = sender;
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
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var command = new LoginUserCommand(model.Email, model.Password);
            var authResponse = await _sender.Send(command, ct);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authResponse.UserId.ToString()),
                new(ClaimTypes.Name, authResponse.FullName),
                new(ClaimTypes.Email, authResponse.Email),
                new(ClaimTypes.Role, authResponse.Role),
                new("JwtToken", authResponse.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            TempData["SuccessMessage"] = $"Welcome back, {authResponse.FullName}!";

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
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken ct)
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
                model.Role
            );

            var authResponse = await _sender.Send(command, ct);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authResponse.UserId.ToString()),
                new(ClaimTypes.Name, authResponse.FullName),
                new(ClaimTypes.Email, authResponse.Email),
                new(ClaimTypes.Role, authResponse.Role),
                new("JwtToken", authResponse.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = "Registration completed successfully!";
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
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["SuccessMessage"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
