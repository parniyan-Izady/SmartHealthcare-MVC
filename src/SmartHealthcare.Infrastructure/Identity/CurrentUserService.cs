using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SmartHealthcare.Application.Common.Interfaces;
using SmartHealthcare.Domain.Enums;

namespace SmartHealthcare.Infrastructure.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? User?.FindFirst("sub")?.Value
                          ?? User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return Guid.TryParse(idClaim, out var guid) ? guid : null;
        }
    }

    public Guid? DoctorId
    {
        get
        {
            var claim = User?.FindFirst("DoctorId")?.Value;
            return Guid.TryParse(claim, out var guid) ? guid : null;
        }
    }

    public Guid? PatientId
    {
        get
        {
            var claim = User?.FindFirst("PatientId")?.Value;
            return Guid.TryParse(claim, out var guid) ? guid : null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var roleClaim = User?.FindFirst(ClaimTypes.Role)?.Value
                            ?? User?.FindFirst("role")?.Value
                            ?? User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            return Enum.TryParse<UserRole>(roleClaim, true, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(params UserRole[] roles)
    {
        if (User == null) return false;
        return roles.Any(r => User.IsInRole(r.ToString())) || (Role.HasValue && roles.Contains(Role.Value));
    }
}
