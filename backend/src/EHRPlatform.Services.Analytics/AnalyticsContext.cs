using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Analytics.Features.Analytics.Domain;

namespace EHRPlatform.Services.Analytics;

public class AnalyticsContext : BaseDbContext
{
    public AnalyticsContext(DbContextOptions<AnalyticsContext> options) : base(options) { }

    public DbSet<AnalyticsMetric> Metrics { get; set; } = null!;
    public DbSet<Dashboard> Dashboards { get; set; } = null!;
    public DbSet<DashboardWidget> DashboardWidgets { get; set; } = null!;
    public DbSet<Report> Reports { get; set; } = null!;
    public DbSet<ReportExecution> ReportExecutions { get; set; } = null!;
    public DbSet<EventMetric> EventMetrics { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AnalyticsMetric>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.MetricName);
            e.HasIndex(x => x.Category);
            e.HasIndex(x => new { x.PeriodStart, x.PeriodEnd });
        });

        modelBuilder.Entity<Dashboard>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<DashboardWidget>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Dashboard).WithMany(d => d.DashboardWidgets).HasForeignKey(x => x.DashboardId);
        });

        modelBuilder.Entity<Report>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => x.Schedule);
        });

        modelBuilder.Entity<ReportExecution>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Report).WithMany(r => r.Executions).HasForeignKey(x => x.ReportId);
            e.HasIndex(x => x.ExecutedAt).IsDescending();
        });

        modelBuilder.Entity<EventMetric>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.EventType);
            e.HasIndex(x => x.OccurredAt).IsDescending();
        });
    }
}
