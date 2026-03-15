using MediatR;
using ResumeBuilder.Application.Common.Models;
namespace ResumeBuilder.Application.Features.Auth.Commands.Register;
public record RegisterCommand(string FirstName, string LastName, string Email, string Password, string ConfirmPassword) : IRequest<Result<string>>;
