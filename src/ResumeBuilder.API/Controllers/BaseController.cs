using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.API.Models;
using ResumeBuilder.Application.Common.Interfaces;
namespace ResumeBuilder.API.Controllers;
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    private ISender? _mediator;
    private ICurrentUserService? _currentUser;
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    protected ICurrentUserService CurrentUser => _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();
    protected Guid GetCurrentUserId() => CurrentUser.UserId ?? throw new Domain.Exceptions.UnauthorizedException("User not authenticated.");
    protected IActionResult OkResponse<T>(T data, string? message = null) => Ok(ApiResponse<T>.Ok(data, message));
    protected IActionResult CreatedResponse<T>(T data, string? message = null) => StatusCode(201, ApiResponse<T>.Created(data, message));
}
