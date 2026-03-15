namespace ResumeBuilder.Application.Features.Admin.DTOs;
public class DashboardStatsDto { public int TotalResumes { get; set; } public int PublishedResumes { get; set; } public int TotalTemplates { get; set; } public int NewResumesThisMonth { get; set; } public List<MonthlyStatDto> MonthlyResumeStats { get; set; } = new(); }
public class MonthlyStatDto { public string Month { get; set; } = ""; public int Count { get; set; } }
public class AdminUserListDto { public Guid Id { get; set; } public string FullName { get; set; } = ""; public string Email { get; set; } = ""; public string? ProfilePicture { get; set; } public string Role { get; set; } = ""; public string Status { get; set; } = ""; public bool IsEmailConfirmed { get; set; } public DateTime CreatedAt { get; set; } public DateTime? LastLoginAt { get; set; } public int ResumeCount { get; set; } public string? BanReason { get; set; } }
public class BanUserDto { public string Reason { get; set; } = ""; }
public class SendNotificationDto { public Guid? UserId { get; set; } public string Title { get; set; } = ""; public string Message { get; set; } = ""; public string Type { get; set; } = "System"; }
