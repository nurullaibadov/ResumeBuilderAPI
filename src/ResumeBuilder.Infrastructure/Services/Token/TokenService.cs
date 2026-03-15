using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Infrastructure.Persistence;
using ResumeBuilder.Infrastructure.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ResumeBuilder.Infrastructure.Services.Token;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwt;
    public TokenService(IOptions<JwtSettings> jwt) => _jwt = jwt.Value;

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        var token = new JwtSecurityToken(issuer: _jwt.Issuer, audience: _jwt.Audience, claims: claims, expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var p = new TokenValidationParameters { ValidateAudience = false, ValidateIssuer = false, ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.SecretKey)), ValidateLifetime = false };
        var principal = new JwtSecurityTokenHandler().ValidateToken(token, p, out var securityToken);
        if (securityToken is not JwtSecurityToken jwt || !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) throw new SecurityTokenException("Invalid token.");
        return principal;
    }
}
