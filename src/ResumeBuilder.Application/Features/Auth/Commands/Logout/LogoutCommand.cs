using MediatR;
namespace ResumeBuilder.Application.Features.Auth.Commands.Logout;
public record LogoutCommand(Guid UserId) : IRequest<bool>;
