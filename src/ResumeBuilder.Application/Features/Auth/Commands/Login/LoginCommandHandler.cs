using MediatR;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Auth.DTOs;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Domain.Exceptions;
namespace ResumeBuilder.Application.Features.Auth.Commands.Login;
public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identity;
    private readonly ITokenService _token;
    public LoginCommandHandler(IIdentityService identity, ITokenService token) { _identity = identity; _token = token; }
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await _identity.FindByEmailAsync(request.Email) ?? throw new UnauthorizedException("Invalid email or password.");
        if (user.IsDeleted) throw new UnauthorizedException("Account not found.");
        if (user.Status == UserStatus.Banned) throw new UnauthorizedException($"Account banned: {user.BanReason}");
        if (!await _identity.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedException("Invalid email or password.");
        if (!await _identity.IsEmailConfirmedAsync(user)) throw new UnauthorizedException("Please verify your email first.");
        var roles = await _identity.GetUserRolesAsync(user);
        var accessToken = _token.GenerateAccessToken(user, roles);
        var refreshToken = _token.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = request.RememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7);
        user.LastLoginAt = DateTime.UtcNow;
        await _identity.UpdateUserAsync(user);
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(60), User = new UserInfoDto { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, FullName = user.FullName, Email = user.Email!, ProfilePicture = user.ProfilePicture, Role = roles.FirstOrDefault() ?? "User", IsEmailConfirmed = user.EmailConfirmed } };
    }
}
