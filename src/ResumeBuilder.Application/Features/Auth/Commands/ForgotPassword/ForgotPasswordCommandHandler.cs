using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
namespace ResumeBuilder.Application.Features.Auth.Commands.ForgotPassword;
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public ForgotPasswordCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByEmailAsync(request.Email);
        if (user != null && !user.IsDeleted) { var token = await _identity.GeneratePasswordResetTokenAsync(user); await _email.SendPasswordResetEmailAsync(user.Email!, user.FullName, token, ct); }
        return "If an account exists, a reset link has been sent.";
    }
}
