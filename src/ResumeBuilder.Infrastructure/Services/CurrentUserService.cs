using Microsoft.AspNetCore.Http;
using ResumeBuilder.Application.Common.Interfaces;
using System.Security.Claims;

namespace ResumeBuilder.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    public CurrentUserService(IHttpContextAccessor http) => _http = http;
    public Guid? UserId { get { var v = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); return Guid.TryParse(v, out var id) ? id : null; } }
    public string? Email => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public bool IsAdmin => _http.HttpContext?.User?.IsInRole("Admin") == true || _http.HttpContext?.User?.IsInRole("SuperAdmin") == true;
    public bool IsSuperAdmin => _http.HttpContext?.User?.IsInRole("SuperAdmin") == true;
}
