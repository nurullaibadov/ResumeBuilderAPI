using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.Features.Templates.Commands.CreateTemplate;
using ResumeBuilder.Application.Features.Templates.Commands.DeleteTemplate;
using ResumeBuilder.Application.Features.Templates.DTOs;
using ResumeBuilder.Application.Features.Templates.Queries.GetAllTemplates;
namespace ResumeBuilder.API.Controllers;
public class TemplatesController : BaseController
{
    [HttpGet, AllowAnonymous] public async Task<IActionResult> GetAll([FromQuery] bool? isActive = true, [FromQuery] string? category = null, CancellationToken ct = default) { var r = await Mediator.Send(new GetAllTemplatesQuery(isActive, category), ct); return OkResponse(r); }
    [HttpPost, Authorize(Roles = "Admin,SuperAdmin")] public async Task<IActionResult> Create([FromBody] CreateTemplateDto dto, CancellationToken ct) { var r = await Mediator.Send(new CreateTemplateCommand(dto.Name, dto.Description, dto.ThumbnailUrl, dto.PreviewUrl, dto.Category, dto.IsActive, dto.IsPremium, dto.CssStyles, dto.HtmlStructure, dto.SortOrder), ct); return CreatedResponse(r, "Template created."); }
    [HttpDelete("{id:guid}"), Authorize(Roles = "Admin,SuperAdmin")] public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) { await Mediator.Send(new DeleteTemplateCommand(id), ct); return OkResponse(true, "Template deleted."); }
}
