using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Infrastructure.Persistence;
using ResumeBuilder.Infrastructure.Settings;

namespace ResumeBuilder.Infrastructure.Services.Email;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;
    private readonly ApplicationDbContext _context;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, ApplicationDbContext context)
    { _settings = settings.Value; _logger = logger; _context = context; }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken ct = default)
    {
        var log = new EmailLog { To = to, Subject = subject, Body = body, SentAt = DateTime.UtcNow };
        try
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            var b = new BodyBuilder();
            if (isHtml) b.HtmlBody = body; else b.TextBody = body;
            msg.Body = b.ToMessageBody();
            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls, ct);
            if (!string.IsNullOrEmpty(_settings.SmtpUsername)) await client.AuthenticateAsync(_settings.SmtpUsername, _settings.SmtpPassword, ct);
            await client.SendAsync(msg, ct);
            await client.DisconnectAsync(true, ct);
            log.IsSuccessful = true;
        }
        catch (Exception ex) { log.IsSuccessful = false; log.ErrorMessage = ex.Message; _logger.LogError(ex, "Email send failed to {To}", to); }
        finally { _context.EmailLogs.Add(log); await _context.SaveChangesAsync(ct); }
    }

    private string Link(string path, string email, string token) => $"{_settings.FrontendBaseUrl}/{path}?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
    private string Html(string title, string content) => $"<html><body style='font-family:Arial;max-width:600px;margin:auto'><div style='background:#4F46E5;padding:20px;text-align:center'><h1 style='color:white'>Resume Builder</h1></div><div style='padding:30px'><h2>{title}</h2>{content}</div><div style='padding:15px;text-align:center;font-size:12px;background:#f3f4f6'>&copy; {DateTime.UtcNow.Year} Resume Builder</div></body></html>";

    public async Task SendWelcomeEmailAsync(string to, string fullName, string token, CancellationToken ct = default) { var url = Link("verify-email", to, token); await SendEmailAsync(to, "Welcome - Verify Email", Html("Welcome!", $"<p>Hi <b>{fullName}</b>!</p><p><a href='{url}' style='background:#4F46E5;color:white;padding:10px 20px;text-decoration:none;border-radius:4px'>Verify Email</a></p><p><a href='{url}'>{url}</a></p>"), true, ct); }
    public async Task SendPasswordResetEmailAsync(string to, string fullName, string token, CancellationToken ct = default) { var url = Link("reset-password", to, token); await SendEmailAsync(to, "Password Reset", Html("Reset Password", $"<p>Hi <b>{fullName}</b>!</p><p><a href='{url}' style='background:#EF4444;color:white;padding:10px 20px;text-decoration:none;border-radius:4px'>Reset Password</a></p>"), true, ct); }
    public async Task SendPasswordChangedNotificationAsync(string to, string fullName, CancellationToken ct = default) => await SendEmailAsync(to, "Password Changed", Html("Password Changed", $"<p>Hi <b>{fullName}</b>, your password was changed at {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC.</p>"), true, ct);
    public async Task SendBanNotificationAsync(string to, string fullName, string reason, CancellationToken ct = default) => await SendEmailAsync(to, "Account Suspended", Html("Account Suspended", $"<p>Hi <b>{fullName}</b>.</p><blockquote style='border-left:4px solid #EF4444;padding-left:16px'>{reason}</blockquote>"), true, ct);
    public async Task SendAccountUnbanNotificationAsync(string to, string fullName, CancellationToken ct = default) => await SendEmailAsync(to, "Account Reactivated", Html("Reactivated", $"<p>Hi <b>{fullName}</b>, your account is active again! <a href='{_settings.FrontendBaseUrl}/login'>Log in</a></p>"), true, ct);
    public async Task SendAdminNotificationAsync(string to, string subject, string message, CancellationToken ct = default) => await SendEmailAsync(to, $"[Admin] {subject}", Html(subject, $"<p>{message}</p>"), true, ct);
}
