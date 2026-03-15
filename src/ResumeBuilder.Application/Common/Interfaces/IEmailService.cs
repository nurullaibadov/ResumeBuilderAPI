namespace ResumeBuilder.Application.Common.Interfaces;
public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default);
    Task SendWelcomeEmailAsync(string to, string fullName, string token, CancellationToken ct = default);
    Task SendPasswordResetEmailAsync(string to, string fullName, string token, CancellationToken ct = default);
    Task SendPasswordChangedNotificationAsync(string to, string fullName, CancellationToken ct = default);
    Task SendBanNotificationAsync(string to, string fullName, string reason, CancellationToken ct = default);
    Task SendAccountUnbanNotificationAsync(string to, string fullName, CancellationToken ct = default);
    Task SendAdminNotificationAsync(string to, string subject, string message, CancellationToken ct = default);
}
