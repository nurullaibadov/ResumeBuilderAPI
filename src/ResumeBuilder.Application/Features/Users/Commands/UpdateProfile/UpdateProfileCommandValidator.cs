using FluentValidation;
namespace ResumeBuilder.Application.Features.Users.Commands.UpdateProfile;
public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{ public UpdateProfileCommandValidator() { RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50); RuleFor(x => x.LastName).NotEmpty().MaximumLength(50); } }
