using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class EducationsController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public EducationsController(IApplicationDbContext ctx) => _ctx = ctx;
    private async Task<Domain.Entities.Resume> GetResumeOwned(Guid resumeId, CancellationToken ct) { var r = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (r.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); return r; }

    [HttpPost("resumes/{resumeId:guid}/educations")]
    public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] CreateEducationDto dto, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = new Education { ResumeId = resumeId, SchoolName = dto.SchoolName, Degree = dto.Degree, FieldOfStudy = dto.FieldOfStudy, Location = dto.Location, StartDate = dto.StartDate, EndDate = dto.IsCurrentlyStudying ? null : dto.EndDate, IsCurrentlyStudying = dto.IsCurrentlyStudying, Grade = dto.Grade, Description = dto.Description, SortOrder = dto.SortOrder };
        _ctx.Educations.Add(e); await _ctx.SaveChangesAsync(ct);
        return CreatedResponse(new EducationDto { Id = e.Id, SchoolName = e.SchoolName, Degree = e.Degree, FieldOfStudy = e.FieldOfStudy, Location = e.Location, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrentlyStudying = e.IsCurrentlyStudying, Grade = e.Grade, Description = e.Description, SortOrder = e.SortOrder });
    }

    [HttpPut("resumes/{resumeId:guid}/educations/{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid resumeId, [FromRoute] Guid id, [FromBody] CreateEducationDto dto, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = await _ctx.Educations.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Education", id);
        e.SchoolName = dto.SchoolName; e.Degree = dto.Degree; e.FieldOfStudy = dto.FieldOfStudy; e.Location = dto.Location; e.StartDate = dto.StartDate; e.EndDate = dto.IsCurrentlyStudying ? null : dto.EndDate; e.IsCurrentlyStudying = dto.IsCurrentlyStudying; e.Grade = dto.Grade; e.Description = dto.Description; e.SortOrder = dto.SortOrder;
        await _ctx.SaveChangesAsync(ct);
        return OkResponse(new EducationDto { Id = e.Id, SchoolName = e.SchoolName, Degree = e.Degree, FieldOfStudy = e.FieldOfStudy, Location = e.Location, StartDate = e.StartDate, EndDate = e.EndDate, IsCurrentlyStudying = e.IsCurrentlyStudying, Grade = e.Grade, Description = e.Description, SortOrder = e.SortOrder });
    }

    [HttpDelete("resumes/{resumeId:guid}/educations/{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid resumeId, [FromRoute] Guid id, CancellationToken ct)
    {
        await GetResumeOwned(resumeId, ct);
        var e = await _ctx.Educations.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Education", id);
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct);
        return OkResponse(true, "Deleted.");
    }
}
