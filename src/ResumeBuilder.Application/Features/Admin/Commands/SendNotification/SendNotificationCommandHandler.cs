using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Enums;
namespace ResumeBuilder.Application.Features.Admin.Commands.SendNotification;
public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, bool>
{
    private readonly IApplicationDbContext _ctx;
    private readonly IEmailService _email;
    private readonly IIdentityService _identity;
    public SendNotificationCommandHandler(IApplicationDbContext ctx, IEmailService email, IIdentityService identity) { _ctx = ctx; _email = email; _identity = identity; }
    public async Task<bool> Handle(SendNotificationCommand r, CancellationToken ct)
    {
        var type = Enum.TryParse<NotificationType>(r.Type, true, out var nt) ? nt : NotificationType.System;
        var users = r.UserId.HasValue ? new[] { await _identity.FindByIdAsync(r.UserId.Value) }.Where(u => u != null).Select(u => u!).ToList() : (await _identity.GetAllActiveUsersAsync(ct)).ToList();
        foreach (var user in users)
        {
            if (type == NotificationType.System || type == NotificationType.Both) _ctx.UserNotifications.Add(new UserNotification { UserId = user.Id, Title = r.Title, Message = r.Message, Type = type });
            if (type == NotificationType.Email || type == NotificationType.Both) await _email.SendAdminNotificationAsync(user.Email!, r.Title, r.Message, ct);
        }
        await _ctx.SaveChangesAsync(ct); return true;
    }
}
