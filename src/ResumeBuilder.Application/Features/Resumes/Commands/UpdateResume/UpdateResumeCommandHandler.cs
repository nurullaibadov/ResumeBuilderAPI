using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Resumes.Commands.UpdateResume;
public class UpdateResumeCommandHandler : IRequestHandler<UpdateResumeCommand, ResumeDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    public UpdateResumeCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<ResumeDetailDto> Handle(UpdateResumeCommand r, CancellationToken ct)
    {
        var resume = await _ctx.Resumes.FirstOrDefaultAsync(x => x.Id == r.ResumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", r.ResumeId);
        if (resume.UserId != r.UserId) throw new ForbiddenAccessException();
        resume.Title = r.Title; resume.Summary = r.Summary; resume.JobTitle = r.JobTitle; resume.TemplateId = r.TemplateId; resume.Phone = r.Phone; resume.Email = r.Email; resume.Address = r.Address; resume.City = r.City; resume.Country = r.Country; resume.PostalCode = r.PostalCode; resume.Website = r.Website; resume.LinkedIn = r.LinkedIn; resume.GitHub = r.GitHub;
        await _ctx.SaveChangesAsync(ct);
        return new ResumeDetailDto { Id = resume.Id, Title = resume.Title, Summary = resume.Summary, JobTitle = resume.JobTitle, Status = resume.Status.ToString(), IsPublic = resume.IsPublic, UserId = resume.UserId, TemplateId = resume.TemplateId, Phone = resume.Phone, Email = resume.Email, Address = resume.Address, City = resume.City, Country = resume.Country, PostalCode = resume.PostalCode, Website = resume.Website, LinkedIn = resume.LinkedIn, GitHub = resume.GitHub, CreatedAt = resume.CreatedAt, UpdatedAt = resume.UpdatedAt };
    }
}
