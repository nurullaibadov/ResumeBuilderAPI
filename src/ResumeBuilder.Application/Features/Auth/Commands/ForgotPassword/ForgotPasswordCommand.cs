using MediatR;
namespace ResumeBuilder.Application.Features.Auth.Commands.ForgotPassword;
public record ForgotPasswordCommand(string Email) : IRequest<string>;
