using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Resumes.Commands.PublishResume;
public class PublishResumeCommandHandler : IRequestHandler<PublishResumeCommand, string>
{
    private readonly IApplicationDbContext _ctx;
    public PublishResumeCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<string> Handle(PublishResumeCommand r, CancellationToken ct)
    {
        var resume = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == r.ResumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", r.ResumeId);
        if (resume.UserId != r.UserId) throw new ForbiddenAccessException();
        resume.Status = ResumeStatus.Published;
        if (r.MakePublic) { resume.IsPublic = true; if (string.IsNullOrEmpty(resume.PublicSlug)) resume.PublicSlug = resume.Id.ToString("N")[..8].ToLower(); }
        await _ctx.SaveChangesAsync(ct);
        return resume.IsPublic ? $"Resume published. Public URL: /resumes/public/{resume.PublicSlug}" : "Resume published.";
    }
}
