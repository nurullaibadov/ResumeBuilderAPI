using MediatR;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Application.Common.Interfaces;
using ResumeBuilder.Application.Features.Admin.DTOs;
using ResumeBuilder.Domain.Enums;
namespace ResumeBuilder.Application.Features.Admin.Queries.GetDashboardStats;
public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    private readonly IApplicationDbContext _ctx;
    public GetDashboardStatsQueryHandler(IApplicationDbContext ctx) => _ctx = ctx;
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery r, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var totalResumes = await _ctx.Resumes.CountAsync(ct);
        var published = await _ctx.Resumes.CountAsync(x => x.Status == ResumeStatus.Published, ct);
        var newThisMonth = await _ctx.Resumes.CountAsync(x => x.CreatedAt >= startOfMonth, ct);
        var totalTemplates = await _ctx.Templates.CountAsync(ct);
        var months = Enumerable.Range(0, 6).Select(i => now.AddMonths(-i)).Select(d => new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc)).OrderBy(d => d).ToList();
        var monthlyStats = new List<MonthlyStatDto>();
        foreach (var m in months) { var next = m.AddMonths(1); var count = await _ctx.Resumes.CountAsync(x => x.CreatedAt >= m && x.CreatedAt < next, ct); monthlyStats.Add(new MonthlyStatDto { Month = m.ToString("MMM yyyy"), Count = count }); }
        return new DashboardStatsDto { TotalResumes = totalResumes, PublishedResumes = published, NewResumesThisMonth = newThisMonth, TotalTemplates = totalTemplates, MonthlyResumeStats = monthlyStats };
    }
}
