using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Users.DTOs;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Users.Queries.GetUserById;
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto>
{
    private readonly IIdentityService _identity;
    private readonly IApplicationDbContext _ctx;
    public GetUserByIdQueryHandler(IIdentityService identity, IApplicationDbContext ctx) { _identity = identity; _ctx = ctx; }
    public async Task<UserDetailDto> Handle(GetUserByIdQuery r, CancellationToken ct)
    {
        var user = await _identity.FindByIdAsync(r.UserId) ?? throw new NotFoundException("User", r.UserId);
        var roles = await _identity.GetUserRolesAsync(user);
        var count = await _ctx.Resumes.CountAsync(x => x.UserId == user.Id, ct);
        return new UserDetailDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email!, PhoneNumber = user.PhoneNumber, Bio = user.Bio, Address = user.Address, City = user.City, Country = user.Country, Website = user.Website, LinkedIn = user.LinkedIn, GitHub = user.GitHub, ProfilePicture = user.ProfilePicture, Role = roles.FirstOrDefault() ?? "User", Status = user.Status.ToString(), IsEmailConfirmed = user.EmailConfirmed, CreatedAt = user.CreatedAt, LastLoginAt = user.LastLoginAt, ResumeCount = count, BanReason = user.BanReason, BannedAt = user.BannedAt };
    }
}
