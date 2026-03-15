using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class ExperiencesController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public ExperiencesController(IApplicationDbContext ctx) => _ctx = ctx;
    private async Task<Domain.Entities.Resume> GetResumeOwned(Guid resumeId, CancellationToken ct) { var r = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (r.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); return r; }

    [HttpPost("resumes/{resumeId:guid}/experiences")]
    public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] CreateExperienceDto dto, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = new Experience { ResumeId = resumeId, CompanyName = dto.CompanyName, JobTitle = dto.JobTitle, Location = dto.Location, EmploymentType = dto.EmploymentType, StartDate = dto.StartDate, EndDate = dto.IsCurrentJob ? null : dto.EndDate, IsCurrentJob = dto.IsCurrentJob, Description = dto.Description, SortOrder = dto.SortOrder };
        _ctx.Experiences.Add(e); await _ctx.SaveChangesAsync(ct);
        return CreatedResponse(new ExperienceDto { Id = e.Id, CompanyName = e.CompanyName, JobTitle = e.JobTitle, Location = e.Location, EmploymentType = e.EmploymentType, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrentJob = e.IsCurrentJob, Description = e.Description, SortOrder = e.SortOrder });
    }

    [HttpPut("resumes/{resumeId:guid}/experiences/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid resumeId, [FromRoute] Guid id, [FromBody] CreateExperienceDto dto, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = await _ctx.Experiences.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Experience", id);
        e.CompanyName = dto.CompanyName; e.JobTitle = dto.JobTitle; e.Location = dto.Location; e.EmploymentType = dto.EmploymentType; e.StartDate = dto.StartDate; e.EndDate = dto.IsCurrentJob ? null : dto.EndDate; e.IsCurrentJob = dto.IsCurrentJob; e.Description = dto.Description; e.SortOrder = dto.SortOrder;
        await _ctx.SaveChangesAsync(ct);
        return OkResponse(new ExperienceDto { Id = e.Id, CompanyName = e.CompanyName, JobTitle = e.JobTitle, Location = e.Location, EmploymentType = e.EmploymentType, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrentJob = e.IsCurrentJob, Description = e.Description, SortOrder = e.SortOrder });
    }

    [HttpDelete("resumes/{resumeId:guid}/experiences/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid resumeId, [FromRoute] Guid id, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = await _ctx.Experiences.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Experience", id);
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct);
        return OkResponse(true, "Deleted.");
    }
}
