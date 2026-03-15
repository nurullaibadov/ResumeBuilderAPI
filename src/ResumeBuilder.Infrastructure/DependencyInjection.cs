using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Infrastructure.Identity;
using ResumeBuilder.Infrastructure.Persistence;
using ResumeBuilder.Infrastructure.Services;
using ResumeBuilder.Infrastructure.Services.Email;
using ResumeBuilder.Infrastructure.Services.Token;
using ResumeBuilder.Infrastructure.Settings;
using System.Text;

namespace ResumeBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection("JwtSettings"));
        services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

        services.AddDbContext<ApplicationDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(p => p.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<AppIdentityUser, IdentityRole<Guid>>(o => {
            o.Password.RequireDigit = true; o.Password.RequireLowercase = true;
            o.Password.RequireUppercase = true; o.Password.RequireNonAlphanumeric = true;
            o.Password.RequiredLength = 8; o.User.RequireUniqueEmail = true;
            o.SignIn.RequireConfirmedEmail = true;
            o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            o.Lockout.MaxFailedAccessAttempts = 5;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        var jwt = config.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.AddAuthentication(o => {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => {
            o.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),
                ValidateIssuer = true, ValidIssuer = jwt.Issuer,
                ValidateAudience = true, ValidAudience = jwt.Audience,
                ValidateLifetime = true, ClockSkew = TimeSpan.Zero
            };
            o.Events = new JwtBearerEvents {
                OnAuthenticationFailed = ctx => {
                    if (ctx.Exception is SecurityTokenExpiredException) ctx.Response.Headers["Token-Expired"] = "true";
                    return Task.CompletedTask;
                }
            };
        });

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddHttpContextAccessor();
        return services;
    }
}
