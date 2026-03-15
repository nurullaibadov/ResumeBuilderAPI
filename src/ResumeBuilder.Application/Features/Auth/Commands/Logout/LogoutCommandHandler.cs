using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
namespace ResumeBuilder.Application.Features.Auth.Commands.Logout;
public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IIdentityService _identity;
    public LogoutCommandHandler(IIdentityService identity) => _identity = identity;
    public async Task<bool> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(request.UserId);
        if (user == null) return false;
        user.RefreshToken = null; user.RefreshTokenExpiryTime = null;
        await _identity.UpdateUserAsync(user);
        return true;
    }
}
