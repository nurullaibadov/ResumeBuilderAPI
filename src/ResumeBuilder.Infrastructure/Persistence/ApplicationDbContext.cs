using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Enums;

namespace ResumeBuilder.Infrastructure.Persistence;

// Identity user that extends IdentityUser - lives in Infrastructure
public class AppIdentityUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
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
}

public class ApplicationDbContext : IdentityDbContext<AppIdentityUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Education> Educations => Set<Education>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<ResumeSkill> ResumeSkills => Set<ResumeSkill>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<ResumeLanguage> ResumeLanguages => Set<ResumeLanguage>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<Template> Templates => Set<Template>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppIdentityUser>(e => e.ToTable("Users"));
        builder.Entity<IdentityRole<Guid>>(e => e.ToTable("Roles"));
        builder.Entity<IdentityUserRole<Guid>>(e => e.ToTable("UserRoles"));
        builder.Entity<IdentityUserClaim<Guid>>(e => e.ToTable("UserClaims"));
        builder.Entity<IdentityUserLogin<Guid>>(e => e.ToTable("UserLogins"));
        builder.Entity<IdentityRoleClaim<Guid>>(e => e.ToTable("RoleClaims"));
        builder.Entity<IdentityUserToken<Guid>>(e => e.ToTable("UserTokens"));

        // Resume - maps UserId to AppIdentityUser
        builder.Entity<Resume>(e => {
            e.Property(r => r.Title).IsRequired().HasMaxLength(100);
            e.Property(r => r.Summary).HasMaxLength(2000);
            e.Property(r => r.PublicSlug).HasMaxLength(50);
            e.HasIndex(r => r.PublicSlug).IsUnique().HasFilter("[PublicSlug] IS NOT NULL");
            e.HasQueryFilter(r => !r.IsDeleted);
            // No navigation to ApplicationUser since it's not an EF entity
        });

        builder.Entity<Education>(e => {
            e.Property(x => x.SchoolName).IsRequired().HasMaxLength(150);
            e.Property(x => x.Degree).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Resume).WithMany(r => r.Educations).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<Experience>(e => {
            e.Property(x => x.CompanyName).IsRequired().HasMaxLength(150);
            e.Property(x => x.JobTitle).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Resume).WithMany(r => r.Experiences).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<Skill>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasIndex(x => x.Name).IsUnique();
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<ResumeSkill>(e => {
            e.HasIndex(x => new { x.ResumeId, x.SkillId }).IsUnique();
            e.HasOne(x => x.Resume).WithMany(r => r.ResumeSkills).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Skill).WithMany(s => s.ResumeSkills).HasForeignKey(x => x.SkillId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Language>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<ResumeLanguage>(e => {
            e.HasIndex(x => new { x.ResumeId, x.LanguageId }).IsUnique();
            e.HasOne(x => x.Resume).WithMany(r => r.ResumeLanguages).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Language).WithMany(l => l.ResumeLanguages).HasForeignKey(x => x.LanguageId).OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Project>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasOne(x => x.Resume).WithMany(r => r.Projects).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<Certificate>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(150);
            e.HasOne(x => x.Resume).WithMany(r => r.Certificates).HasForeignKey(x => x.ResumeId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<Template>(e => {
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<UserNotification>(e => {
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        builder.Entity<AppIdentityUser>(e => {
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(50);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(50);
            e.Property(x => x.Bio).HasMaxLength(500);
            e.Property(x => x.RefreshToken).HasMaxLength(500);
            e.Property(x => x.BanReason).HasMaxLength(500);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Domain.Common.AuditableEntity>())
        {
            if (entry.State == EntityState.Added) entry.Entity.CreatedAt = DateTime.UtcNow;
            else if (entry.State == EntityState.Modified) entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
