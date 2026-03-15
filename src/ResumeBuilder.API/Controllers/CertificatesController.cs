using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class CertificatesController : BaseController
{
    private readonly IApplicationDbContext _ctx;
    public CertificatesController(IApplicationDbContext ctx) => _ctx = ctx;
    private async Task<Domain.Entities.Resume> GetResumeOwned(Guid id, CancellationToken ct) { var r = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", id); if (r.UserId != GetCurrentUserId()) throw new ForbiddenAccessException(); return r; }
    [HttpPost("resumes/{resumeId:guid}/certificates")] public async Task<IActionResult> Add([FromRoute] Guid resumeId, [FromBody] CreateCertificateDto dto, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var c = new Certificate { ResumeId = resumeId, Name = dto.Name, IssuingOrganization = dto.IssuingOrganization, IssueDate = dto.IssueDate, ExpirationDate = dto.DoesNotExpire ? null : dto.ExpirationDate, DoesNotExpire = dto.DoesNotExpire, CredentialId = dto.CredentialId, CredentialUrl = dto.CredentialUrl, SortOrder = dto.SortOrder }; _ctx.Certificates.Add(c); await _ctx.SaveChangesAsync(ct); return CreatedResponse(new CertificateDto { Id = c.Id, Name = c.Name, IssuingOrganization = c.IssuingOrganization, IssueDate = c.IssueDate, ExpirationDate = c.ExpirationDate, DoesNotExpire = c.DoesNotExpire, CredentialId = c.CredentialId, CredentialUrl = c.CredentialUrl, SortOrder = c.SortOrder }); }
    [HttpPut("resumes/{resumeId:guid}/certificates/{id:guid}")] public async Task<IActionResult> Update([FromRoute] Guid resumeId, [FromRoute] Guid id, [FromBody] CreateCertificateDto dto, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var c = await _ctx.Certificates.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Certificate", id); c.Name = dto.Name; c.IssuingOrganization = dto.IssuingOrganization; c.IssueDate = dto.IssueDate; c.ExpirationDate = dto.DoesNotExpire ? null : dto.ExpirationDate; c.DoesNotExpire = dto.DoesNotExpire; c.CredentialId = dto.CredentialId; c.CredentialUrl = dto.CredentialUrl; c.SortOrder = dto.SortOrder; await _ctx.SaveChangesAsync(ct); return OkResponse(new CertificateDto { Id = c.Id, Name = c.Name, IssuingOrganization = c.IssuingOrganization, IssueDate = c.IssueDate, ExpirationDate = c.ExpirationDate, DoesNotExpire = c.DoesNotExpire, CredentialId = c.CredentialId, CredentialUrl = c.CredentialUrl, SortOrder = c.SortOrder }); }
    [HttpDelete("resumes/{resumeId:guid}/certificates/{id:guid}")] public async Task<IActionResult> Delete([FromRoute] Guid resumeId, [FromRoute] Guid id, CancellationToken ct) { await GetResumeOwned(resumeId, ct); var c = await _ctx.Certificates.FirstOrDefaultAsync(x => x.Id == id && x.ResumeId == resumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Certificate", id); c.IsDeleted = true; c.DeletedAt = DateTime.UtcNow; await _ctx.SaveChangesAsync(ct); return OkResponse(true, "Deleted."); }
}
