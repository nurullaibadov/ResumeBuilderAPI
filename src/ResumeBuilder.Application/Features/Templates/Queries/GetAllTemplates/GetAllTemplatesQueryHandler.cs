using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Templates.DTOs;
namespace ResumeBuilder.Application.Features.Templates.Queries.GetAllTemplates;
public class GetAllTemplatesQueryHandler : IRequestHandler<GetAllTemplatesQuery, List<TemplateListDto>>
{
    private readonly IApplicationDbContext _ctx;
    public GetAllTemplatesQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<List<TemplateListDto>> Handle(GetAllTemplatesQuery r, CancellationToken ct)
    {
        var q = _ctx.Templates.Where(t => !t.IsDeleted);
        if (r.IsActive.HasValue) q = q.Where(t => t.IsActive == r.IsActive.Value);
        if (!string.IsNullOrEmpty(r.Category) && Enum.TryParse<Domain.Enums.TemplateCategory>(r.Category, true, out var cat)) q = q.Where(t => t.Category == cat);
        return await q.OrderBy(t => t.SortOrder).ThenBy(t => t.Name).Select(t => new TemplateListDto { Id = t.Id, Name = t.Name, Description = t.Description, ThumbnailUrl = t.ThumbnailUrl, Category = t.Category.ToString(), IsActive = t.IsActive, IsPremium = t.IsPremium, UsageCount = t.UsageCount, SortOrder = t.SortOrder }).ToListAsync(ct);
    }
}
