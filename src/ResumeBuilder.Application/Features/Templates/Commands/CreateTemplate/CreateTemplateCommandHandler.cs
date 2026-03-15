using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Templates.DTOs;
using ResumeBuilder.Domain.Entities;
namespace ResumeBuilder.Application.Features.Templates.Commands.CreateTemplate;
public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, TemplateDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    public CreateTemplateCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<TemplateDetailDto> Handle(CreateTemplateCommand r, CancellationToken ct)
    {
        var t = new Template { Name = r.Name, Description = r.Description, ThumbnailUrl = r.ThumbnailUrl, PreviewUrl = r.PreviewUrl, Category = r.Category, IsActive = r.IsActive, IsPremium = r.IsPremium, CssStyles = r.CssStyles, HtmlStructure = r.HtmlStructure, SortOrder = r.SortOrder };
        _ctx.Templates.Add(t); await _ctx.SaveChangesAsync(ct);
        return new TemplateDetailDto { Id = t.Id, Name = t.Name, Description = t.Description, ThumbnailUrl = t.ThumbnailUrl, PreviewUrl = t.PreviewUrl, Category = t.Category.ToString(), IsActive = t.IsActive, IsPremium = t.IsPremium, CssStyles = t.CssStyles, HtmlStructure = t.HtmlStructure, SortOrder = t.SortOrder, CreatedAt = t.CreatedAt };
    }
}
