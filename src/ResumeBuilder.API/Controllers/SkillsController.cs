using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class SkillsController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public SkillsController(IApplicationDbContext ctx) => _ctx = ctx;
    [HttpGet, AllowAnonymous] public async Task<IActionResult> GetAll([FromQuery] string? category, CancellationToken ct) { var q = _ctx.Skills.Where(s => s.IsActive); if (!string.IsNullOrEmpty(category)) q = q.Where(s => s.Category == category); return OkResponse(await q.OrderBy(s => s.Category).ThenBy(s => s.Name).Select(s => new { s.Id, s.Name, s.Category }).ToListAsync(ct)); }
    [HttpPost("resumes/{resumeId:guid}/skills")] public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] AddSkillToResumeDto dto, CancellationToken ct) { var resume = await _ctx.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId && !r.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (resume.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); var skill = await _ctx.Skills.FirstOrDefaultAsync(s => s.Id == dto.SkillId && s.IsActive, ct) ?? throw new NotFoundException("Skill", dto.SkillId); if (await _ctx.ResumeSkills.AnyAsync(rs => rs.ResumeId == resumeId && rs.SkillId == dto.SkillId, ct)) throw new ConflictException("Skill already added."); var rs2 = new ResumeSkill { ResumeId = resumeId, SkillId = dto.SkillId, Level = dto.Level, SortOrder = dto.SortOrder }; _ctx.ResumeSkills.Add(rs2); await _ctx.SaveChangesAsync(ct); return CreatedResponse(new ResumeSkillDto { Id = rs2.Id, SkillId = skill.Id, SkillName = skill.Name, Category = skill.Category, Level = dto.Level.ToString(), SortOrder = dto.SortOrder }); }
    [HttpDelete("resumes/{resumeId:guid}/skills/{rsId:guid}")] public async Task<IActionResult> Remove([FromRoute] Guid resumeId, [FromRoute] Guid rsId, CancellationToken ct) { var resume = await _ctx.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId && !r.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (resume.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); var rs = await _ctx.ResumeSkills.FirstOrDefaultAsync(r => r.Id == rsId && r.ResumeId == resumeId, ct) ?? throw new NotFoundException("ResumeSkill", rsId); _ctx.ResumeSkills.Remove(rs); await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Skill removed."); }
}
