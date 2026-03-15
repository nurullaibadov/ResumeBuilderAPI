using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Templates.Commands.DeleteTemplate;
public class DeleteTemplateCommandHandler : IRequestHandler<DeleteTemplateCommand, bool>
{
    private readonly IApplicationDbContext _ctx;
    public DeleteTemplateCommandHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<bool> Handle(DeleteTemplateCommand r, CancellationToken ct)
    {
        var t = await _ctx.Templates.FindAsync(new object[] { r.TemplateId }, ct) ?? throw new NotFoundException("Template", r.TemplateId);
        t.IsDeleted = true; t.DeletedAt = DateTime.UtcNow;
        await _ctx.SaveChangesAsync(ct); return true;
    }
}
