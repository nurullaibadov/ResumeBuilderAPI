using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Domain.Entities;
namespace ResumeBuilder.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Resume> Resumes { get; }
    DbSet<Education> Educations { get; }
    DbSet<Experience> Experiences { get; }
    DbSet<Skill> Skills { get; }
    DbSet<ResumeSkill> ResumeSkills { get; }
    DbSet<Language> Languages { get; }
    DbSet<ResumeLanguage> ResumeLanguages { get; }
    DbSet<Project> Projects { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<Template> Templates { get; }
    DbSet<UserNotification> UserNotifications { get; }
    DbSet<SystemLog> SystemLogs { get; }
    DbSet<EmailLog> EmailLogs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
