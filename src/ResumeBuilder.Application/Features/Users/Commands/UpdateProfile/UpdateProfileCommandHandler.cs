using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Users.DTOs;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Users.Commands.UpdateProfile;
public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UserDetailDto>
{
    private readonly IIdentityService _identity;
    public UpdateProfileCommandHandler(IIdentityService identity) => _identity = identity;
    public async Task<UserDetailDto> Handle(UpdateProfileCommand r, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(r.UserId) ?? throw new NotFoundException("User", r.UserId);
        user.FirstName = r.FirstName; user.LastName = r.LastName; user.PhoneNumber = r.PhoneNumber; user.Bio = r.Bio; user.Address = r.Address; user.City = r.City; user.Country = r.Country; user.Website = r.Website; user.LinkedIn = r.LinkedIn; user.GitHub = r.GitHub; user.UpdatedAt = DateTime.UtcNow;
        await _identity.UpdateUserAsync(user);
        var roles = await _identity.GetUserRolesAsync(user);
        return new UserDetailDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email!, PhoneNumber = user.PhoneNumber, Bio = user.Bio, Address = user.Address, City = user.City, Country = user.Country, Website = user.Website, LinkedIn = user.LinkedIn, GitHub = user.GitHub, ProfilePicture = user.ProfilePicture, Role = roles.FirstOrDefault() ?? "User", Status = user.Status.ToString(), IsEmailConfirmed = user.EmailConfirmed, CreatedAt = user.CreatedAt };
    }
}
