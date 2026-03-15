using MediatR;
using ResumeBuilder.Application.Features.Templates.DTOs;
namespace ResumeBuilder.Application.Features.Templates.Queries.GetAllTemplates;
public record GetAllTemplatesQuery(bool? IsActive, string? Category) : IRequest<List<TemplateListDto>>;
