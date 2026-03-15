using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Resumes.DTOs;
using ResumeBuilder.Domain.Entities;
namespace ResumeBuilder.Application.Features.Resumes.Commands.CreateResume;
public class CreateResumeCommandHandler : IRequestHandler<CreateResumeCommand, ResumeDetailDto>
{
    private readonly IApplicationDbContext _ctx;
    public CreateResumeCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<ResumeDetailDto> Handle(CreateResumeCommand r, CancellationToken ct)
    {
        var resume = new Resume { UserId = r.UserId, Title = r.Title, Summary = r.Summary, JobTitle = r.JobTitle, TemplateId = r.TemplateId, Phone = r.Phone, Email = r.Email, Address = r.Address, City = r.City, Country = r.Country, PostalCode = r.PostalCode, Website = r.Website, LinkedIn = r.LinkedIn, GitHub = r.GitHub };
        _ctx.Resumes.Add(resume);
        await _ctx.SaveChangesAsync(ct);
        return new ResumeDetailDto { Id = resume.Id, Title = resume.Title, Summary = resume.Summary, JobTitle = resume.JobTitle, Status = resume.Status.ToString(), IsPublic = resume.IsPublic, UserId = resume.UserId, TemplateId = resume.TemplateId, Phone = resume.Phone, Email = resume.Email, Address = resume.Address, City = resume.City, Country = resume.Country, PostalCode = resume.PostalCode, Website = resume.Website, LinkedIn = resume.LinkedIn, GitHub = resume.GitHub, CreatedAt = resume.CreatedAt };
    }
}
