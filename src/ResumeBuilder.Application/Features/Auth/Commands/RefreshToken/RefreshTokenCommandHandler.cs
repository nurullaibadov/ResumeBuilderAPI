using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Auth.DTOs;
using ResumeBuilder.Domain.Exceptions;
using System.Security.Claims;
namespace ResumeBuilder.Application.Features.Auth.Commands.RefreshToken;
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _token;
    public RefreshTokenCommandHandler(IIdentityService identity, ITokenService token) { _identity = identity; _token = token; }
    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = _token.GetPrincipalFromExpiredToken(request.AccessToken);
        var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedException("Invalid token.");
        if (!Guid.TryParse(userIdStr, out var userId)) throw new UnauthorizedException("Invalid token.");
        var user = await _identity.FindByIdAsync(userId) ?? throw new UnauthorizedException("User not found.");
        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow) throw new UnauthorizedException("Invalid or expired refresh token.");
        var roles = await _identity.GetUserRolesAsync(user);
        var newAccess = _token.GenerateAccessToken(user, roles);
        var newRefresh = _token.GenerateRefreshToken();
        user.RefreshToken = newRefresh; user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _identity.UpdateUserAsync(user);
        return new AuthResponseDto { AccessToken = newAccess, RefreshToken = newRefresh, ExpiresAt = DateTime.UtcNow.AddMinutes(60), User = new UserInfoDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email!, ProfilePicture = user.ProfilePicture, Role = roles.FirstOrDefault() ?? "User", IsEmailConfirmed = user.EmailConfirmed } };
    }
}
