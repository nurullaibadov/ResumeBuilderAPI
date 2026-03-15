using MediatR;
using ResumeBuilder.Application.Features.Users.DTOs;
namespace ResumeBuilder.Application.Features.Users.Queries.GetUserById;
public record GetUserByIdQuery(Guid UserId) : IRequest<UserDetailDto>;
