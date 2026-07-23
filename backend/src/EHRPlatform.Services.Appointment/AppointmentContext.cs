using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;

namespace EHRPlatform.Services.Appointment;

/// <summary>
/// DbContext for Appointment Service.
/// </summary>
public class AppointmentContext : BaseDbContext
{
    public AppointmentContext(DbContextOptions<AppointmentContext> options) : base(options) { }

    public DbSet<Domain.Appointment> Appointments { get; set; } = null!;
    public DbSet<AppointmentReminder> AppointmentReminders { get; set; } = null!;
    public DbSet<ProviderAvailability> ProviderAvailability { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Appointment configuration
        modelBuilder.Entity<Domain.Appointment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => e.ScheduledStart).IsDescending();
            entity.HasIndex(e => new { e.ProviderId, e.ScheduledStart });
            entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("Scheduled");
            entity.Property(e => e.AppointmentType).HasMaxLength(50);
        });

        // AppointmentReminder configuration
        modelBuilder.Entity<AppointmentReminder>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Appointment)
                .WithMany(a => a.Reminders)
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.AppointmentId);
            entity.HasIndex(e => e.ReminderTime);
            entity.Property(e => e.Method).HasMaxLength(50);
        });

        // ProviderAvailability configuration
        modelBuilder.Entity<ProviderAvailability>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProviderId);
            entity.HasIndex(e => new { e.ProviderId, e.SlotStart, e.SlotEnd });
            entity.Property(e => e.RecurrencePattern).HasMaxLength(50);
        });
    }
}
