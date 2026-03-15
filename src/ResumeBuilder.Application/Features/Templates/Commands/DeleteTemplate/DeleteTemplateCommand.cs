using MediatR;
namespace ResumeBuilder.Application.Features.Templates.Commands.DeleteTemplate;
public record DeleteTemplateCommand(Guid TemplateId) : IRequest<bool>;
