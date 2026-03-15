using MediatR;
using ResumeBuilder.Application.Features.Templates.DTOs;
using ResumeBuilder.Domain.Enums;
namespace ResumeBuilder.Application.Features.Templates.Commands.CreateTemplate;
public record CreateTemplateCommand(string Name, string? Description, string? ThumbnailUrl, string? PreviewUrl, TemplateCategory Category, bool IsActive, bool IsPremium, string? CssStyles, string? HtmlStructure, int SortOrder) : IRequest<TemplateDetailDto>;
