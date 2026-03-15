namespace ResumeBuilder.Application.Features.Auth.DTOs;
public class LoginDto { public string Email { get; set; } = ""; public string Password { get; set; } = ""; public bool RememberMe { get; set; } }
public class RegisterDto { public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string Email { get; set; } = ""; public string Password { get; set; } = ""; public string ConfirmPassword { get; set; } = ""; }
public class ForgotPasswordDto { public string Email { get; set; } = ""; }
public class ResetPasswordDto { public string Email { get; set; } = ""; public string Token { get; set; } = ""; public string NewPassword { get; set; } = ""; public string ConfirmPassword { get; set; } = ""; }
public class ChangePasswordDto { public string CurrentPassword { get; set; } = ""; public string NewPassword { get; set; } = ""; public string ConfirmPassword { get; set; } = ""; }
public class RefreshTokenDto { public string AccessToken { get; set; } = ""; public string RefreshToken { get; set; } = ""; }
public class VerifyEmailDto { public string Email { get; set; } = ""; public string Token { get; set; } = ""; }
public class AuthResponseDto { public string AccessToken { get; set; } = ""; public string RefreshToken { get; set; } = ""; public DateTime ExpiresAt { get; set; } public UserInfoDto User { get; set; } = null!; }
public class UserInfoDto { public Guid Id { get; set; } public string FirstName { get; set; } = ""; public string LastName { get; set; } = ""; public string FullName { get; set; } = ""; public string Email { get; set; } = ""; public string? ProfilePicture { get; set; } public string Role { get; set; } = ""; public bool IsEmailConfirmed { get; set; } }
