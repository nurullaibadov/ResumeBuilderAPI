using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Common.Models;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Enums;
using ResumeBuilder.Infrastructure.Persistence;

namespace ResumeBuilder.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<AppIdentityUser> _userManager;

    public IdentityService(UserManager<AppIdentityUser> userManager) => _userManager = userManager;

    // Maps AppIdentityUser -> Domain ApplicationUser
    private static ApplicationUser? Map(AppIdentityUser? u) => u == null ? null : new ApplicationUser
    {
        Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, UserName = u.UserName, Email = u.Email,
        EmailConfirmed = u.EmailConfirmed, PhoneNumber = u.PhoneNumber, ProfilePicture = u.ProfilePicture,
        Bio = u.Bio, Address = u.Address, City = u.City, Country = u.Country, Website = u.Website,
        LinkedIn = u.LinkedIn, GitHub = u.GitHub, Status = u.Status, Role = u.Role, CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt, LastLoginAt = u.LastLoginAt, IsDeleted = u.IsDeleted, DeletedAt = u.DeletedAt,
        RefreshToken = u.RefreshToken, RefreshTokenExpiryTime = u.RefreshTokenExpiryTime,
        BanReason = u.BanReason, BannedAt = u.BannedAt
    };

    // Maps Domain ApplicationUser -> AppIdentityUser
    private static void MapBack(ApplicationUser src, AppIdentityUser dst)
    {
        dst.FirstName = src.FirstName; dst.LastName = src.LastName; dst.PhoneNumber = src.PhoneNumber;
        dst.ProfilePicture = src.ProfilePicture; dst.Bio = src.Bio; dst.Address = src.Address;
        dst.City = src.City; dst.Country = src.Country; dst.Website = src.Website; dst.LinkedIn = src.LinkedIn;
        dst.GitHub = src.GitHub; dst.Status = src.Status; dst.Role = src.Role; dst.UpdatedAt = src.UpdatedAt;
        dst.LastLoginAt = src.LastLoginAt; dst.IsDeleted = src.IsDeleted; dst.DeletedAt = src.DeletedAt;
        dst.RefreshToken = src.RefreshToken; dst.RefreshTokenExpiryTime = src.RefreshTokenExpiryTime;
        dst.BanReason = src.BanReason; dst.BannedAt = src.BannedAt;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email) => Map(await _userManager.FindByEmailAsync(email));
    public async Task<ApplicationUser?> FindByIdAsync(Guid id) => Map(await _userManager.FindByIdAsync(id.ToString()));

    public async Task<IList<ApplicationUser>> GetAllActiveUsersAsync(CancellationToken ct = default)
    {
        var users = await _userManager.Users.Where(u => !u.IsDeleted && u.Status == UserStatus.Active).ToListAsync(ct);
        return users.Select(u => Map(u)!).ToList();
    }

    public async Task<(Result Result, Guid UserId)> CreateUserAsync(string email, string password, string firstName, string lastName)
    {
        var user = new AppIdentityUser
        {
            UserName = email, Email = email, FirstName = firstName, LastName = lastName,
            Status = UserStatus.PendingVerification, Role = UserRole.User, CreatedAt = DateTime.UtcNow
        };
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded) { await _userManager.AddToRoleAsync(user, "User"); return (Result.Success(), user.Id); }
        return (Result.Failure(result.Errors.Select(e => e.Description)), Guid.Empty);
    }

    public async Task<Result> UpdateUserAsync(ApplicationUser user)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        if (iu == null) return Result.Failure("User not found.");
        MapBack(user, iu);
        var ir = await _userManager.UpdateAsync(iu);
        return ir.Succeeded ? Result.Success() : Result.Failure(ir.Errors.Select(e => e.Description));
    }

    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        return iu != null && await _userManager.CheckPasswordAsync(iu, password);
    }

    public async Task<Result> ChangePasswordAsync(ApplicationUser user, string current, string newPwd)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        if (iu == null) return Result.Failure("User not found.");
        var ir = await _userManager.ChangePasswordAsync(iu, current, newPwd);
        return ir.Succeeded ? Result.Success() : Result.Failure(ir.Errors.Select(e => e.Description));
    }

    public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        return iu == null ? Array.Empty<string>() : await _userManager.GetRolesAsync(iu);
    }

    public async Task<bool> IsEmailConfirmedAsync(ApplicationUser user)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        return iu != null && iu.EmailConfirmed;
    }

    public async Task<Result> ConfirmEmailAsync(ApplicationUser user, string token)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        if (iu == null) return Result.Failure("User not found.");
        var ir = await _userManager.ConfirmEmailAsync(iu, token);
        return ir.Succeeded ? Result.Success() : Result.Failure(ir.Errors.Select(e => e.Description));
    }

    public async Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        return iu == null ? "" : await _userManager.GenerateEmailConfirmationTokenAsync(iu);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        return iu == null ? "" : await _userManager.GeneratePasswordResetTokenAsync(iu);
    }

    public async Task<Result> ResetPasswordAsync(ApplicationUser user, string token, string newPwd)
    {
        var iu = await _userManager.FindByIdAsync(user.Id.ToString());
        if (iu == null) return Result.Failure("User not found.");
        var ir = await _userManager.ResetPasswordAsync(iu, token, newPwd);
        return ir.Succeeded ? Result.Success() : Result.Failure(ir.Errors.Select(e => e.Description));
    }
}
