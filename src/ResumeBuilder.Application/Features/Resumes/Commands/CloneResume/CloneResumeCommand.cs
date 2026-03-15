using MediatR;
namespace ResumeBuilder.Application.Features.Resumes.Commands.CloneResume;
public record CloneResumeCommand(Guid ResumeId, Guid UserId) : IRequest<Guid>;
