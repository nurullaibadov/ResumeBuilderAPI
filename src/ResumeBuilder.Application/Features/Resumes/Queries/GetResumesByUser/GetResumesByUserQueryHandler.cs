using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Common.Models;
using ResumeBuilder.Application.Features.Resumes.DTOs;
namespace ResumeBuilder.Application.Features.Resumes.Queries.GetResumesByUser;
public class GetResumesByUserQueryHandler : IRequestHandler<GetResumesByUserQuery, PaginatedList<ResumeListDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetResumesByUserQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<PaginatedList<ResumeListDto>> Handle(GetResumesByUserQuery r, CancellationToken ct)
    {
        var query = _ctx.Resumes.Include(x => x.Template).Where(x => x.UserId == r.UserId && !x.IsDeleted).OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).Select(x => new ResumeListDto { Id = x.Id, Title = x.Title, JobTitle = x.JobTitle, Status = x.Status.ToString(), IsPublic = x.IsPublic, PublicSlug = x.PublicSlug, ViewCount = x.ViewCount, TemplateName = x.Template != null ? x.Template.Name : null, CreatedAt = x.CreatedAt, UpdatedAt = x.UpdatedAt });
        return await PaginatedList<ResumeListDto>.CreateAsync(query, r.PageNumber, r.PageSize);
    }
}
