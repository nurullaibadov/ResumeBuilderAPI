using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Admin.Commands.UnbanUser;
public class UnbanUserCommandHandler : IRequestHandler<UnbanUserCommand, bool>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public UnbanUserCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<bool> Handle(UnbanUserCommand r, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(r.UserId) ?? throw new NotFoundException("User", r.UserId);
        if (user.Status != UserStatus.Banned) throw new BadRequestException("User is not banned.");
        user.Status = UserStatus.Active; user.BanReason = null; user.BannedAt = null;
        await _identity.UpdateUserAsync(user);
        await _email.SendAccountUnbanNotificationAsync(user.Email!, user.FullName, ct);
        return true;
    }
}
