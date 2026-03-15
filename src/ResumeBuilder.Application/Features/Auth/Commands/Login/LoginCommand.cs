using MediatR;
using ResumeBuilder.Application.Features.Auth.DTOs;
namespace ResumeBuilder.Application.Features.Auth.Commands.Login;
public record LoginCommand(string Email, string Password, bool RememberMe) : IRequest<AuthResponseDto>;
