using MediatR;
namespace ResumeBuilder.Application.Features.Admin.Commands.BanUser;
public record BanUserCommand(Guid UserId, string Reason, Guid AdminId) : IRequest<bool>;
