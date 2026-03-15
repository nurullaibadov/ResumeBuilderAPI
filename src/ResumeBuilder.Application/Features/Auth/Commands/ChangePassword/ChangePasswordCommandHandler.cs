using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Auth.Commands.ChangePassword;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, string>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public ChangePasswordCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<string> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(request.UserId) ?? throw new NotFoundException("User not found.");
        var result = await _identity.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors));
        await _email.SendPasswordChangedNotificationAsync(user.Email!, user.FullName, ct);
        user.RefreshToken = null; user.RefreshTokenExpiryTime = null;
        await _identity.UpdateUserAsync(user);
        return "Password changed successfully.";
    }
}
