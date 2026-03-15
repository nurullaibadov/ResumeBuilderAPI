using MediatR;
using ResumeBuilder.Application.Features.Resumes.DTOs;
namespace ResumeBuilder.Application.Features.Resumes.Commands.CreateResume;
public record CreateResumeCommand(Guid UserId, string Title, string? Summary, string? JobTitle, Guid? TemplateId, string? Phone, string? Email, string? Address, string? City, string? Country, string? PostalCode, string? Website, string? LinkedIn, string? GitHub) : IRequest<ResumeDetailDto>;
