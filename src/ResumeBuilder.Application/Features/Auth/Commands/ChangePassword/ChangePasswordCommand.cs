using MediatR;
namespace ResumeBuilder.Application.Features.Auth.Commands.ChangePassword;
public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword, string ConfirmPassword) : IRequest<string>;
