using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Admin.Commands.BanUser;
using ResumeBuilder.Application.Features.Admin.Commands.SendNotification;
using ResumeBuilder.Application.Features.Admin.Commands.UnbanUser;
using ResumeBuilder.Application.Features.Admin.DTOs;
using ResumeBuilder.Application.Features.Admin.Queries.GetDashboardStats;
using ResumeBuilder.Application.Features.Users.DTOs;
namespace ResumeBuilder.API.Controllers;
[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/v1/admin")]
[ApiController]
[Produces("application/json")]
public class AdminController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    private readonly IIdentityService _identity;
    public AdminController(IApplicationDbContext ctx, IIdentityService identity) { _ctx = ctx; _identity = identity; }

    [HttpGet("dashboard")] public async Task<IActionResult> Dashboard(CancellationToken ct) => OkResponse(await Mediator.Send(new GetDashboardStatsQuery(), ct));

    [HttpGet("users")] public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string? search = null, CancellationToken ct = default)
    {
        var users = await _identity.GetAllActiveUsersAsync(ct);
        var q = users.AsQueryable();
        if (!string.IsNullOrEmpty(search)) { var s = search.ToLower(); q = q.Where(u => (u.FirstName + " " + u.LastName).ToLower().Contains(s) || u.Email!.ToLower().Contains(s)); }
        var total = q.Count();
        var items = q.OrderByDescending(u => u.CreatedAt).Skip((page - 1) * size).Take(size).Select(u => new AdminUserListDto { Id = u.Id, FullName = u.FullName, Email = u.Email!, ProfilePicture = u.ProfilePicture, Role = u.Role.ToString(), Status = u.Status.ToString(), IsEmailConfirmed = u.EmailConfirmed, CreatedAt = u.CreatedAt, LastLoginAt = u.LastLoginAt, BanReason = u.BanReason }).ToList();
        return Ok(new PagedApiResponse<AdminUserListDto> { Success = true, Items = items, PageNumber = page, TotalPages = (int)Math.Ceiling(total / (double)size), TotalCount = total });
    }

    [HttpGet("users/{userId:guid}")] public async Task<IActionResult> GetUser([FromRoute] Guid userId, CancellationToken ct) { var user = await _identity.FindByIdAsync(userId); if (user == null) return NotFound(); var roles = await _identity.GetUserRolesAsync(user); var count = await _ctx.Resumes.CountAsync(r => r.UserId == userId, ct); return OkResponse(new UserDetailDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email!, PhoneNumber = user.PhoneNumber, Bio = user.Bio, Address = user.Address, City = user.City, Country = user.Country, Website = user.Website, LinkedIn = user.LinkedIn, GitHub = user.GitHub, ProfilePicture = user.ProfilePicture, Role = roles.FirstOrDefault() ?? "User", Status = user.Status.ToString(), IsEmailConfirmed = user.EmailConfirmed, CreatedAt = user.CreatedAt, LastLoginAt = user.LastLoginAt, ResumeCount = count, BanReason = user.BanReason, BannedAt = user.BannedAt }); }

    [HttpPost("users/{userId:guid}/ban")] public async Task<IActionResult> BanUser([FromRoute] Guid userId, [FromBody] BanUserDto dto, CancellationToken ct) { await Mediator.Send(new BanUserCommand(userId, dto.Reason, GetCurrentUserId()), ct); return OkResponse(true, "User banned."); }
    [HttpPost("users/{userId:guid}/unban")] public async Task<IActionResult> UnbanUser([FromRoute] Guid userId, CancellationToken ct) { await Mediator.Send(new UnbanUserCommand(userId, GetCurrentUserId()), ct); return OkResponse(true, "User unbanned."); }
    [HttpPost("notifications/send")] public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto, CancellationToken ct) { await Mediator.Send(new SendNotificationCommand(dto.UserId, dto.Title, dto.Message, dto.Type), ct); return OkResponse(true, "Notification sent."); }

    [HttpGet("resumes")] public async Task<IActionResult> GetResumes([FromQuery] int page = 1, [FromQuery] int size = 20, [FromQuery] string? search = null, CancellationToken ct = default) { var q = _ctx.Resumes.IgnoreQueryFilters().Include(r => r.User).Include(r => r.Template).Where(r => !r.IsDeleted); if (!string.IsNullOrEmpty(search)) q = q.Where(r => r.Title.Contains(search) || r.User.Email!.Contains(search)); var total = await q.CountAsync(ct); var items = await q.OrderByDescending(r => r.CreatedAt).Skip((page - 1) * size).Take(size).Select(r => new { r.Id, r.Title, Status = r.Status.ToString(), r.IsPublic, r.ViewCount, r.CreatedAt, User = new { r.User.Id, r.User.FullName, r.User.Email }, Template = r.Template != null ? new { r.Template.Id, r.Template.Name } : null }).ToListAsync(ct); return Ok(new PagedApiResponse<object> { Success = true, Items = items.Cast<object>().ToList(), PageNumber = page, TotalPages = (int)Math.Ceiling(total / (double)size), TotalCount = total }); }

    [HttpGet("templates")] public async Task<IActionResult> GetTemplates(CancellationToken ct) => OkResponse(await _ctx.Templates.IgnoreQueryFilters().OrderBy(t => t.SortOrder).ToListAsync(ct));
    [HttpPatch("templates/{id:guid}/toggle-active")] public async Task<IActionResult> ToggleTemplate([FromRoute] Guid id, CancellationToken ct) { var t = await _ctx.Templates.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, ct); if (t == null) return NotFound(); t.IsActive = !t.IsActive; await _ctx.SaveChangesAsync(ct); return OkResponse(new { t.Id, t.IsActive }); }

    [HttpGet("logs/emails")] public async Task<IActionResult> EmailLogs([FromQuery] int page = 1, [FromQuery] int size = 50, CancellationToken ct = default) { var total = await _ctx.EmailLogs.CountAsync(ct); var items = await _ctx.EmailLogs.OrderByDescending(e => e.SentAt).Skip((page - 1) * size).Take(size).Select(e => new { e.Id, e.To, e.Subject, e.IsSuccessful, e.SentAt, e.ErrorMessage, e.EmailType }).ToListAsync(ct); return Ok(new PagedApiResponse<object> { Success = true, Items = items.Cast<object>().ToList(), PageNumber = page, TotalPages = (int)Math.Ceiling(total / (double)size), TotalCount = total }); }
    [HttpGet("logs/system"), Authorize(Roles = "SuperAdmin")] public async Task<IActionResult> SystemLogs([FromQuery] int page = 1, [FromQuery] int size = 50, CancellationToken ct = default) { var total = await _ctx.SystemLogs.CountAsync(ct); var items = await _ctx.SystemLogs.OrderByDescending(l => l.CreatedAt).Skip((page - 1) * size).Take(size).ToListAsync(ct); return Ok(new PagedApiResponse<object> { Success = true, Items = items.Cast<object>().ToList(), PageNumber = page, TotalPages = (int)Math.Ceiling(total / (double)size), TotalCount = total }); }
}
