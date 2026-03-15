using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Features.Resumes.Commands.CloneResume;
using ResumeBuilder.Application.Features.Resumes.Commands.CreateResume;
using ResumeBuilder.Application.Features.Resumes.Commands.DeleteResume;
using ResumeBuilder.Application.Features.Resumes.Commands.PublishResume;
using ResumeBuilder.Application.Features.Resumes.Commands.UpdateResume;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Application.Features.Resumes.Queries.GetResumeById;
using ResumeBuilder.Application.Features.Resumes.Queries.GetResumeBySlug;
using ResumeBuilder.Application.Features.Resumes.Queries.GetResumesByUser;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class ResumesController : BaseController
{
    [HttpGet] public async Task<IActionResult> GetMyResumes([FromQuery] int page = 1, [FromQuery] int size = 10, CancellationToken ct = default) { var r = await Mediator.Send(new GetResumesByUserQuery(GetCurrentUserId(), page, size), ct); return Ok(new PagedApiResponse<ResumeListDto> { Success = true, Items = r.Items, PageNumber = r.PageNumber, TotalPages = r.TotalPages, TotalCount = r.TotalCount, HasPreviousPage = r.HasPreviousPage, HasNextPage = r.HasNextPage }); }
    [HttpGet("{id:guid}")] public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) { var r = await Mediator.Send(new GetResumeByIdQuery(id, GetCurrentUserId()), ct); return OkResponse(r); }
    [HttpGet("public/{slug}"), AllowAnonymous] public async Task<IActionResult> GetPublic([FromRoute] string slug, CancellationToken ct) { var r = await Mediator.Send(new GetResumeBySlugQuery(slug), ct); return OkResponse(r); }
    [HttpPost] public async Task<IActionResult> Create([FromBody] CreateResumeDto dto, CancellationToken ct) { var r = await Mediator.Send(new CreateResumeCommand(GetCurrentUserId(), dto.Title, dto.Summary, dto.JobTitle, dto.TemplateId, dto.Phone, dto.Email, dto.Address, dto.City, dto.Country, dto.PostalCode, dto.Website, dto.LinkedIn, dto.GitHub), ct); return CreatedResponse(r, "Resume created."); }
    [HttpPut("{id:guid}")] public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateResumeDto dto, CancellationToken ct) { var r = await Mediator.Send(new UpdateResumeCommand(id, GetCurrentUserId(), dto.Title, dto.Summary, dto.JobTitle, dto.TemplateId, dto.Phone, dto.Email, dto.Address, dto.City, dto.Country, dto.PostalCode, dto.Website, dto.LinkedIn, dto.GitHub), ct); return OkResponse(r, "Resume updated."); }
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct) { await Mediator.Send(new DeleteResumeCommand(id, GetCurrentUserId()), ct); return OkResponse(true, "Resume deleted."); }
    [HttpPost("{id:guid}/publish")] public async Task<IActionResult> Publish([FromRoute] Guid id, [FromQuery] bool makePublic = false, CancellationToken ct = default) { var r = await Mediator.Send(new PublishResumeCommand(id, GetCurrentUserId(), makePublic), ct); return OkResponse(r); }
    [HttpPost("{id:guid}/clone")] public async Task<IActionResult> Clone([FromRoute] Guid id, CancellationToken ct) { var r = await Mediator.Send(new CloneResumeCommand(id, GetCurrentUserId()), ct); return CreatedResponse(r, "Resume cloned."); }
}
