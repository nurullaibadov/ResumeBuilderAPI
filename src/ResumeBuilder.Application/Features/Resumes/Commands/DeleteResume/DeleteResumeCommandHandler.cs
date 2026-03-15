using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Resumes.Commands.DeleteResume;
public class DeleteResumeCommandHandler : IRequestHandler<DeleteResumeCommand, bool>
{
    private readonly IApplicationDbContext _ctx;
    public DeleteResumeCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<bool> Handle(DeleteResumeCommand r, CancellationToken ct)
    {
        var resume = await _ctx.Resumes.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == r.ResumeId && !x.IsDeleted, ct) ?? throw new NotFoundException("Resume", r.ResumeId);
        if (!r.IsAdmin && resume.UserId != r.UserId) throw new ForbiddenAccessException();
        resume.IsDeleted = true; resume.DeletedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync(ct);
        return true;
    }
}
