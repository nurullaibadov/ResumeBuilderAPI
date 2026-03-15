using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Common.Interfaces;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class NotificationsController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public NotificationsController(IApplicationDbContext ctx) => _ctx = ctx;
    [HttpGet] public async Task<IActionResult> GetMyNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int size = 20, CancellationToken ct = default) { var uid = GetCurrentUserId(); var q = _ctx.UserNotifications.Where(n => n.UserId == uid && !n.IsDeleted); if (unreadOnly) q = q.Where(n => !n.IsRead); var total = await q.CountAsync(ct); var items = await q.OrderByDescending(n => n.CreatedAt).Skip((page - 1) * size).Take(size).Select(n => new { n.Id, n.Title, n.Message, n.IsRead, n.ReadAt, Type = n.Type.ToString(), n.ActionUrl, n.CreatedAt }).ToListAsync(ct); return Ok(new PagedApiResponse<object> { Success = true, Items = items.Cast<object>().ToList(), PageNumber = page, TotalPages = (int)Math.Ceiling(total / (double)size), TotalCount = total, HasPreviousPage = page > 1, HasNextPage = page < (int)Math.Ceiling(total / (double)size) }); }
    [HttpGet("unread-count")] public async Task<IActionResult> GetUnreadCount(CancellationToken ct) => OkResponse(new { count = await _ctx.UserNotifications.CountAsync(n => n.UserId == GetCurrentUserId() && !n.IsRead && !n.IsDeleted, ct) });
    [HttpPatch("{id:guid}/read")] public async Task<IActionResult> MarkAsRead([FromRoute] Guid id, CancellationToken ct) { var n = await _ctx.UserNotifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == GetCurrentUserId() && !x.IsDeleted, ct); if (n == null) return NotFound(); n.IsRead = true; n.ReadAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Marked as read."); }
    [HttpPatch("read-all")] public async Task<IActionResult> MarkAllAsRead(CancellationToken ct) { var uid = GetCurrentUserId(); var items = await _ctx.UserNotifications.Where(n => n.UserId == uid && !n.IsRead && !n.IsDeleted).ToListAsync(ct); var now = DateTime.UtcNow; foreach (var n in items) { n.IsRead = true; n.ReadAt = now; } await _ctx.SaveChangesAsync(ct); return OkResponse(true, $"Marked {items.Count} as read."); }
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) { var n = await _ctx.UserNotifications.FirstOrDefaultAsync(x => x.Id == id && x.UserId == GetCurrentUserId() && !x.IsDeleted, ct); if (n == null) return NotFound(); n.IsDeleted = true; n.DeletedAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Deleted."); }
}
