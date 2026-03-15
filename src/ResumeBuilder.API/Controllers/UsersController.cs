using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResumeBuilder.Application.Features.Users.Commands.UpdateProfile;
using ResumeBuilder.Application.Features.Users.DTOs;
using ResumeBuilder.Application.Features.Users.Queries.GetUserById;
namespace ResumeBuilder.API.Controllers;
[Authorize]
public class UsersController : BaseController
{
    [HttpGet("me")] public async Task<IActionResult> GetMyProfile(CancellationToken ct) { var r = await Mediator.Send(new GetUserByIdQuery(GetCurrentUserId()), ct); return OkResponse(r); }
    [HttpPut("me")] public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileDto dto, CancellationToken ct) { var r = await Mediator.Send(new UpdateProfileCommand(GetCurrentUserId(), dto.FirstName, dto.LastName, dto.PhoneNumber, dto.Bio, dto.Address, dto.City, dto.Country, dto.Website, dto.LinkedIn, dto.GitHub), ct); return OkResponse(r, "Profile updated."); }
    [HttpGet("{id:guid}")] public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct) { if (id != GetCurrentUserId() && !CurrentUser.IsAdmin) return Forbid(); var r = await Mediator.Send(new GetUserByIdQuery(id), ct); return OkResponse(r); }
}
