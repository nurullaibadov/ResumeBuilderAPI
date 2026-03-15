namespace ResumeBuilder.Domain.Enums;
public enum UserStatus { Active = 1, Inactive = 2, Banned = 3, PendingVerification = 4 }
public enum UserRole { User = 1, Admin = 2, SuperAdmin = 3 }
public enum ResumeStatus { Draft = 1, Published = 2, Archived = 3 }
public enum SkillLevel { Beginner = 1, Elementary = 2, Intermediate = 3, Advanced = 4, Expert = 5 }
public enum LanguageLevel { A1 = 1, A2 = 2, B1 = 3, B2 = 4, C1 = 5, C2 = 6, Native = 7 }
public enum TemplateCategory { Professional = 1, Creative = 2, Simple = 3, Modern = 4, Academic = 5 }
public enum NotificationType { Email = 1, System = 2, Both = 3 }
