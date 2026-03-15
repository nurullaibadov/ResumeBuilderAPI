using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Auth.Commands.ResetPassword;
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, string>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public ResetPasswordCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<string> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User not found.");
        var result = await _identity.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors));
        await _email.SendPasswordChangedNotificationAsync(user.Email!, user.FullName, ct);
        return "Password has been reset successfully.";
    }
}
