using MediatR;
using ResumeBuilder.Application.Features.Users.DTOs;
namespace ResumeBuilder.Application.Features.Users.Commands.UpdateProfile;
public record UpdateProfileCommand(Guid UserId, string FirstName, string LastName, string? PhoneNumber, string? Bio, string? Address, string? City, string? Country, string? Website, string? LinkedIn, string? GitHub) : IRequest<UserDetailDto>;
