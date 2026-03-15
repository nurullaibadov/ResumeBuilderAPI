using ResumeBuilder.Domain.Entities;
using System.Security.Claims;
namespace ResumeBuilder.Application.Common.Interfaces;
public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
