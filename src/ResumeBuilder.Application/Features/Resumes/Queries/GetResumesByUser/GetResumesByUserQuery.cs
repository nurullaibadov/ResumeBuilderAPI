using MediatR;
using ResumeBuilder.Application.Common.Models;
using ResumeBuilder.Application.Features.Resumes.DTOs;
namespace ResumeBuilder.Application.Features.Resumes.Queries.GetResumesByUser;
public record GetResumesByUserQuery(Guid UserId, int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedList<ResumeListDto>>;
