using ResumeBuilder.Domain.Common;
using ResumeBuilder.Domain.Enums;

namespace ResumeBuilder.Domain.Entities;

// Plain user entity - no Identity dependency in Domain layer
public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool EmailConfirmed { get; set; } = false;
    public string? PhoneNumber { get; set; }
    public string? PasswordHash { get; set; }
    public string? ProfilePicture { get; set; }
    public string? Bio { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }
    public UserStatus Status { get; set; } = UserStatus.PendingVerification;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? BanReason { get; set; }
    public DateTime? BannedAt { get; set; }
    public string FullName => $"{FirstName} {LastName}";

    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    public ICollection<UserNotification> Notifications { get; set; } = new List<UserNotification>();
}

public class Resume : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? JobTitle { get; set; }
    public ResumeStatus Status { get; set; } = ResumeStatus.Draft;
    public bool IsPublic { get; set; } = false;
    public string? PublicSlug { get; set; }
    public int ViewCount { get; set; } = 0;
    public Guid UserId { get; set; }
    public Guid? TemplateId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Website { get; set; }
    public string? LinkedIn { get; set; }
    public string? GitHub { get; set; }
    public string? ProfilePicture { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Template? Template { get; set; }
    public ICollection<Education> Educations { get; set; } = new List<Education>();
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<ResumeLanguage> ResumeLanguages { get; set; } = new List<ResumeLanguage>();
    public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
}

public class Education : AuditableEntity
{
    public string SchoolName { get; set; } = string.Empty;
    public string Degree { get; set; } = string.Empty;
    public string? FieldOfStudy { get; set; }
    public string? Location { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentlyStudying { get; set; } = false;
    public string? Grade { get; set; }
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
}

public class Experience : AuditableEntity
{
    public string CompanyName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? EmploymentType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentJob { get; set; } = false;
    public string? Description { get; set; }
    public int SortOrder { get; set; } = 0;
    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
}

public class Skill : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ResumeSkill> ResumeSkills { get; set; } = new List<ResumeSkill>();
}

public class ResumeSkill : BaseEntity
{
    public Guid ResumeId { get; set; }
    public Guid SkillId { get; set; }
    public SkillLevel Level { get; set; } = SkillLevel.Intermediate;
    public int SortOrder { get; set; } = 0;
    public Resume Resume { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}

public class Language : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? NativeName { get; set; }
    public string? Code { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<ResumeLanguage> ResumeLanguages { get; set; } = new List<ResumeLanguage>();
}

public class ResumeLanguage : BaseEntity
{
    public Guid ResumeId { get; set; }
    public Guid LanguageId { get; set; }
    public LanguageLevel Level { get; set; } = LanguageLevel.B1;
    public int SortOrder { get; set; } = 0;
    public Resume Resume { get; set; } = null!;
    public Language Language { get; set; } = null!;
}

public class Project : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Technologies { get; set; }
    public string? Url { get; set; }
    public string? GithubUrl { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsOngoing { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
}

public class Certificate : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? IssuingOrganization { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool DoesNotExpire { get; set; } = false;
    public string? CredentialId { get; set; }
    public string? CredentialUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public Guid ResumeId { get; set; }
    public Resume Resume { get; set; } = null!;
}

public class Template : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PreviewUrl { get; set; }
    public TemplateCategory Category { get; set; } = TemplateCategory.Professional;
    public bool IsActive { get; set; } = true;
    public bool IsPremium { get; set; } = false;
    public int UsageCount { get; set; } = 0;
    public string? CssStyles { get; set; }
    public string? HtmlStructure { get; set; }
    public int SortOrder { get; set; } = 0;
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
}

public class UserNotification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    public NotificationType Type { get; set; } = NotificationType.System;
    public string? ActionUrl { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}

public class SystemLog : BaseEntity
{
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string? UserId { get; set; }
    public string? RequestPath { get; set; }
    public string? IpAddress { get; set; }
}

public class EmailLog : BaseEntity
{
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public string? EmailType { get; set; }
}
