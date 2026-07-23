using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;

namespace EHRPlatform.Services.Appointment.Data;

/// <summary>
/// DbContext for Appointment Service.
/// </summary>
public class AppointmentContext : BaseDbContext
{
    public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options) { }

    public DbSet<Entities.Appointment> Appointments { get; set; } = null!;
    public DbSet<AppointmentReminder> AppointmentReminders { get; set; } = null!;
    public DbSet<ProviderAvailability> ProviderAvailability { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new Configuration.AppointmentConfiguration());
        modelBuilder.ApplyConfiguration(new Configuration.AppointmentReminderConfiguration());
        modelBuilder.ApplyConfiguration(new Configuration.ProviderAvailabilityConfiguration());

        // Apply seeds
        modelBuilder.SeedAppointments();
    }
}
