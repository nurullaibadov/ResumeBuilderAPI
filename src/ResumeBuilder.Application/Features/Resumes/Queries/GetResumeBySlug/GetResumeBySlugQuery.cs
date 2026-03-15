using MediatR;
using ResumeBuilder.Application.Features.Resumes.DTOs;
namespace ResumeBuilder.Application.Features.Resumes.Queries.GetResumeBySlug;
public record GetResumeBySlugQuery(string Slug) : IRequest<ResumeDetailDto>;
