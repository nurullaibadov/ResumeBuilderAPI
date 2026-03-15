using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class LanguagesController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public LanguagesController(IApplicationDbContext ctx) => _ctx = ctx;
    [HttpGet, AllowAnonymous] public async Task<IActionResult> GetAll(CancellationToken ct) => OkResponse(await _ctx.Languages.Where(l => l.IsActive).OrderBy(l => l.Name).Select(l => new { l.Id, l.Name, l.NativeName, l.Code }).ToListAsync(ct));
    [HttpPost("resumes/{resumeId:guid}/languages")] public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] AddLanguageToResumeDto dto, CancellationToken ct) { var resume = await _ctx.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId && !r.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (resume.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); var lang = await _ctx.Languages.FirstOrDefaultAsync(l => l.Id == dto.LanguageId && l.IsActive, ct) ?? throw new NotFoundException("Language", dto.LanguageId); if (await _ctx.ResumeLanguages.AnyAsync(rl => rl.ResumeId == resumeId && rl.LanguageId == dto.LanguageId, ct)) throw new ConflictException("Language already added."); var rl = new ResumeLanguage { ResumeId = resumeId, LanguageId = dto.LanguageId, Level = dto.Level, SortOrder = dto.SortOrder }; _ctx.ResumeLanguages.Add(rl); await _ctx.SaveChangesAsync(ct); return CreatedResponse(new ResumeLanguageDto { Id = rl.Id, LanguageId = lang.Id, LanguageName = lang.Name, Level = dto.Level.ToString(), SortOrder = dto.SortOrder }); }
    [HttpDelete("resumes/{resumeId:guid}/languages/{rlId:guid}")] public async Task<IActionResult> Remove([FromRoute] Guid resumeId, [FromRoute] Guid rlId, CancellationToken ct) { var resume = await _ctx.Resumes.FirstOrDefaultAsync(r => r.Id == resumeId && !r.IsDeleted, ct) ?? throw new NotFoundException("Resume", resumeId); if (resume.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); var rl = await _ctx.ResumeLanguages.FirstOrDefaultAsync(r => r.Id == rlId && r.ResumeId == resumeId, ct) ?? throw new NotFoundException("ResumeLanguage", rlId); _ctx.ResumeLanguages.Remove(rl); await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Language removed."); }
}
