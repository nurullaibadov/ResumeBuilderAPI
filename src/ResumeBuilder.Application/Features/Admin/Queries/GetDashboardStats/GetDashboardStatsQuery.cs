using MediatR;
using ResumeBuilder.Application.Features.Admin.DTOs;
namespace ResumeBuilder.Application.Features.Admin.Queries.GetDashboardStats;
public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;
