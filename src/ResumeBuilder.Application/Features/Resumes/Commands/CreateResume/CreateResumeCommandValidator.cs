using FluentValidation;
namespace ResumeBuilder.Application.Features.Resumes.Commands.CreateResume;
public class CreateResumeCommandValidator : AbstractValidator<CreateResumeCommand>
{ public CreateResumeCommandValidator() { RuleFor(x => x.Title).NotEmpty().MaximumLength(100); } }
