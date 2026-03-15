using MediatR;
namespace ResumeBuilder.Application.Features.Admin.Commands.UnbanUser;
public record UnbanUserCommand(Guid UserId, Guid AdminId) : IRequest<bool>;
