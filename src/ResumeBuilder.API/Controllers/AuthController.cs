using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Features.Auth.Commands.ChangePassword;
using ResumeBuilder.Application.Features.Auth.Commands.ForgotPassword;
using ResumeBuilder.Application.Features.Auth.Commands.Login;
using ResumeBuilder.Application.Features.Auth.Commands.Logout;
using ResumeBuilder.Application.Features.Auth.Commands.RefreshToken;
using ResumeBuilder.Application.Features.Auth.Commands.Register;
using ResumeBuilder.Application.Features.Auth.Commands.ResetPassword;
using ResumeBuilder.Application.Features.Auth.Commands.VerifyEmail;
using ResumeBuilder.Application.Features.Auth.DTOs;
namespace ResumeBuilder.API.Controllers;
public class AuthController : BaseController
{
    [HttpPost("register")] [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct) { var r = await Mediator.Send(new RegisterCommand(dto.FirstName, dto.LastName, dto.Email, dto.Password, dto.ConfirmPassword), ct); return CreatedResponse(r.Data, r.Message); }

    [HttpPost("login")] [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct) { var r = await Mediator.Send(new LoginCommand(dto.Email, dto.Password, dto.RememberMe), ct); return OkResponse(r, "Login successful."); }

    [HttpPost("refresh-token")] [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken ct) { var r = await Mediator.Send(new RefreshTokenCommand(dto.AccessToken, dto.RefreshToken), ct); return OkResponse(r); }

    [HttpPost("logout")] [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct) { await Mediator.Send(new LogoutCommand(GetCurrentUserId()), ct); return OkResponse(true, "Logged out."); }

    [HttpPost("forgot-password")] [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto, CancellationToken ct) { var r = await Mediator.Send(new ForgotPasswordCommand(dto.Email), ct); return OkResponse(r); }

    [HttpPost("reset-password")] [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto, CancellationToken ct) { var r = await Mediator.Send(new ResetPasswordCommand(dto.Email, dto.Token, dto.NewPassword, dto.ConfirmPassword), ct); return OkResponse(r); }

    [HttpPost("change-password")] [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, CancellationToken ct) { var r = await Mediator.Send(new ChangePasswordCommand(GetCurrentUserId(), dto.CurrentPassword, dto.NewPassword, dto.ConfirmPassword), ct); return OkResponse(r); }

    [HttpPost("verify-email")] [AllowAnonymous]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto dto, CancellationToken ct) { var r = await Mediator.Send(new VerifyEmailCommand(dto.Email, dto.Token), ct); return OkResponse(r); }
}
