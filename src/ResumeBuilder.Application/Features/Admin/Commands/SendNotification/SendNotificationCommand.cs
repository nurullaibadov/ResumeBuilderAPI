using MediatR;
namespace ResumeBuilder.Application.Features.Admin.Commands.SendNotification;
public record SendNotificationCommand(Guid? UserId, string Title, string Message, string Type) : IRequest<bool>;
