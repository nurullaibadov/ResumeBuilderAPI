using MediatR;
namespace ResumeBuilder.Application.Features.Resumes.Commands.PublishResume;
public record PublishResumeCommand(Guid ResumeId, Guid UserId, bool MakePublic) : IRequest<string>;
