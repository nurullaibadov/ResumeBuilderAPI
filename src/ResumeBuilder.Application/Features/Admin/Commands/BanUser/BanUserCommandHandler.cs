using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Admin.Commands.BanUser;
public class BanUserCommandHandler : IRequestHandler<BanUserCommand, bool>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public BanUserCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<bool> Handle(BanUserCommand r, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(r.UserId) ?? throw new NotFoundException("User", r.UserId);
        if (user.Status == UserStatus.Banned) throw new BadRequestException("User is already banned.");
        user.Status = UserStatus.Banned; user.BanReason = r.Reason; user.BannedAt = DateTime.UtcNow; user.RefreshToken = null; user.RefreshTokenExpiryTime = null;
        await _identity.UpdateUserAsync(user);
        await _email.SendBanNotificationAsync(user.Email!, user.FullName, r.Reason, ct);
        return true;
    }
}
