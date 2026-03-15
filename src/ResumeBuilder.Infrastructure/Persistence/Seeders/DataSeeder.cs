using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Enums;

namespace ResumeBuilder.Infrastructure.Persistence.Seeders;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<AppIdentityUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        foreach (var role in new[] { "SuperAdmin", "Admin", "User" })
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));

        const string adminEmail = "superadmin@resumebuilder.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new AppIdentityUser { FirstName = "Super", LastName = "Admin", UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, Status = UserStatus.Active, Role = UserRole.SuperAdmin, CreatedAt = DateTime.UtcNow };
            if ((await userManager.CreateAsync(admin, "Admin@123456!")).Succeeded)
            { await userManager.AddToRoleAsync(admin, "SuperAdmin"); await userManager.AddToRoleAsync(admin, "Admin"); }
        }

        if (!await context.Skills.AnyAsync())
        {
            context.Skills.AddRange(new[] { ("C#","Programming"),("ASP.NET Core","Framework"),("JavaScript","Programming"),("TypeScript","Programming"),("React","Framework"),("Angular","Framework"),("Vue.js","Framework"),("Node.js","Runtime"),("Python","Programming"),("Java","Programming"),("SQL Server","Database"),("PostgreSQL","Database"),("MongoDB","Database"),("Redis","Database"),("Docker","DevOps"),("Kubernetes","DevOps"),("Azure","Cloud"),("AWS","Cloud"),("Git","Version Control"),("HTML/CSS","Frontend"),("Project Management","Soft Skill"),("Team Leadership","Soft Skill"),("Communication","Soft Skill"),("Problem Solving","Soft Skill") }.Select(s => new Skill { Name = s.Item1, Category = s.Item2 }));
            await context.SaveChangesAsync();
        }

        if (!await context.Languages.AnyAsync())
        {
            context.Languages.AddRange(new[] { ("English","English","en"),("Turkish","Türkçe","tr"),("Azerbaijani","Azərbaycan dili","az"),("Russian","Русский","ru"),("German","Deutsch","de"),("French","Français","fr"),("Spanish","Español","es"),("Arabic","العربية","ar"),("Chinese","中文","zh"),("Japanese","日本語","ja") }.Select(l => new Language { Name = l.Item1, NativeName = l.Item2, Code = l.Item3 }));
            await context.SaveChangesAsync();
        }

        if (!await context.Templates.AnyAsync())
        {
            context.Templates.AddRange(
                new Template { Name = "Classic Professional", Description = "Clean traditional layout.", Category = TemplateCategory.Professional, IsActive = true, SortOrder = 1 },
                new Template { Name = "Modern Clean", Description = "Contemporary modern design.", Category = TemplateCategory.Modern, IsActive = true, SortOrder = 2 },
                new Template { Name = "Creative Edge", Description = "Bold creative layout.", Category = TemplateCategory.Creative, IsActive = true, SortOrder = 3 },
                new Template { Name = "Simple Minimal", Description = "Minimalist design.", Category = TemplateCategory.Simple, IsActive = true, SortOrder = 4 },
                new Template { Name = "Academic CV", Description = "Comprehensive academic layout.", Category = TemplateCategory.Academic, IsActive = true, SortOrder = 5 });
            await context.SaveChangesAsync();
        }
    }
}
