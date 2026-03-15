using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class ProjectsController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public ProjectsController(IApplicationDbContext ctx) => _ctx = ctx;
    private async Task<Domain.Entities.Resume> GetResumeOwned(Guid id, CancellationToken ct) { var r = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", id); if (r.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); return r; }
    [HttpPost("resumes/{resumeId:guid}/projects")] public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] CreateProjectDto dto, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var p = new Project { ResumeId = resumeId, Name = dto.Name, Description = dto.Description, Technologies = dto.Technologies, Url = dto.Url, GithubUrl = dto.GithubUrl, StartDate = dto.StartDate, EndDate = dto.IsOngoing ? null : dto.EndDate, IsOngoing = dto.IsOngoing, SortOrder = dto.SortOrder }; _ctx.Projects.Add(p); await _ctx.SaveChangesAsync(ct); return CreatedResponse(new ProjectDto { Id = p.Id, Name = p.Name, Description = p.Description, Technologies = p.Technologies, Url = p.Url, GithubUrl = p.GithubUrl, StartDate = p.StartDate, EndDate = p.EndDate, IsOngoing = p.IsOngoing, SortOrder = p.SortOrder }); }
    [HttpPut("resumes/{resumeId:guid}/projects/{id:guid}")] public async Task<IActionResult> Update([FromRoute] Guid resumeId, [FromRoute] Guid id, [FromBody] CreateProjectDto dto, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var p = await _ctx.Projects.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Project", id); p.Name = dto.Name; p.Description = dto.Description; p.Technologies = dto.Technologies; p.Url = dto.Url; p.GithubUrl = dto.GithubUrl; p.StartDate = dto.StartDate; p.EndDate = dto.IsOngoing ? null : dto.EndDate; p.IsOngoing = dto.IsOngoing; p.SortOrder = dto.SortOrder; await _ctx.SaveChangesAsync(ct); return OkResponse(new ProjectDto { Id = p.Id, Name = p.Name, Description = p.Description, Technologies = p.Technologies, Url = p.Url, GithubUrl = p.GithubUrl, StartDate = p.StartDate, EndDate = p.EndDate, IsOngoing = p.IsOngoing, SortOrder = p.SortOrder }); }
    [HttpDelete("resumes/{resumeId:guid}/projects/{id:guid}")] public async Task<IActionResult> Delete([FromRoute] Guid resumeId, [FromRoute] Guid id, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var p = await _ctx.Projects.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Project", id); p.IsDeleted = true; p.DeletedAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Deleted."); }
}
