using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Auth.Commands.VerifyEmail;
public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, string>
{
    private readonly IIdentityService _identity;
    public VerifyEmailCommandHandler(IIdentityService identity) => _identity = identity;
    public async Task<string> Handle(VerifyEmailCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User not found.");
        if (user.EmailConfirmed) return "Email already verified.";
        var result = await _identity.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded) throw new BadRequestException("Invalid or expired token.");
        user.Status = UserStatus.Active;
        await _identity.UpdateUserAsync(user);
        return "Email verified successfully.";
    }
}
