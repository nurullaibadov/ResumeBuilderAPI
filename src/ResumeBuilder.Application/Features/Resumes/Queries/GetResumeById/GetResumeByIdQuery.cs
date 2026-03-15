using MediatR;
using ResumeBuilder.Application.Features.Resumes.DTOs;
namespace ResumeBuilder.Application.Features.Resumes.Queries.GetResumeById;
public record GetResumeByIdQuery(Guid ResumeId, Guid? RequestingUserId) : IRequest<ResumeDetailDto>;
