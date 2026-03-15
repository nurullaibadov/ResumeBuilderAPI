using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Common.Models;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Auth.Commands.Register;
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly IIdentityService _identity;
    private readonly IEmailService _email;
    public RegisterCommandHandler(IIdentityService identity, IEmailService email) { _identity = identity; _email = email; }
    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
    {
        var existing = await _identity.FindByEmailAsync(request.Email);
        if (existing != null) throw new ConflictException("An account with this email already exists.");
        var (result, userId) = await _identity.CreateUserAsync(request.Email, request.Password, request.FirstName, request.LastName);
        if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors));
        var user = await _identity.FindByIdAsync(userId);
        if (user != null) { var token = await _identity.GenerateEmailConfirmationTokenAsync(user); await _email.SendWelcomeEmailAsync(user.Email!, user.FullName, token, ct); }
        return Result<string>.Success("Registration successful. Please check your email to verify your account.");
    }
}
