using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Resumes.Commands.CloneResume;
public class CloneResumeCommandHandler : IRequestHandler<CloneResumeCommand, Guid>
{
    private readonly IApplicationDbContext _ctx;
    public CloneResumeCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<Guid> Handle(CloneResumeCommand r, CancellationToken ct)
    {
        var src = await _ctx.Resumes.Include(x => x.Educations).Include(x => x.Experiences).Include(x => x.ResumeSkills).Include(x => x.Projects).Include(x => x.ResumeLanguages).Include(x => x.Certificates).FirstOrDefaultAsync(x => x.Id == r.ResumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", r.ResumeId);
        if (src.UserId != r.UserId) throw new ForbiddenAccessException();
        var clone = new Resume { UserId = r.UserId, Title = src.Title + " (Copy)", Summary = src.Summary, JobTitle = src.JobTitle, TemplateId = src.TemplateId, Phone = src.Phone, Email = src.Email, Address = src.Address, City = src.City, Country = src.Country, PostalCode = src.PostalCode, Website = src.Website, LinkedIn = src.LinkedIn, GitHub = src.GitHub, Status = ResumeStatus.Draft };
        _ctx.Resumes.Add(clone);
        await _ctx.SaveChangesAsync(ct);
        return clone.Id;
    }
}
