using MediatR;
namespace ResumeBuilder.Application.Features.Resumes.Commands.DeleteResume;
public record DeleteResumeCommand(Guid ResumeId, Guid UserId, bool IsAdmin = false) : IRequest<bool>;
