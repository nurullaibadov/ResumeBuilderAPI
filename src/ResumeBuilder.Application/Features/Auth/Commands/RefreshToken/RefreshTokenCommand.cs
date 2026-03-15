using MediatR;
using ResumeBuilder.Application.Features.Auth.DTOs;
namespace ResumeBuilder.Application.Features.Auth.Commands.RefreshToken;
public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<AuthResponseDto>;
