using MediatR;
namespace ResumeBuilder.Application.Features.Auth.Commands.ResetPassword;
public record ResetPasswordCommand(string Email, string Token, string NewPassword, string ConfirmPassword) : IRequest<string>;
