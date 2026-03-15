using ResumeBuilder.Application.Common.Models;
using ResumeBuilder.Domain.Entities;

namespace ResumeBuilder.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<ApplicationUser?> FindByEmailAsync(string email);
    Task<ApplicationUser?> FindByIdAsync(Guid id);
    Task<IList<ApplicationUser>> GetAllActiveUsersAsync(CancellationToken ct = default);
    Task<(Result Result, Guid UserId)> CreateUserAsync(string email, string password, string firstName, string lastName);
    Task<Result> UpdateUserAsync(ApplicationUser user);
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<Result> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    Task<bool> IsEmailConfirmedAsync(ApplicationUser user);
    Task<Result> ConfirmEmailAsync(ApplicationUser user, string token);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<Result> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
}
