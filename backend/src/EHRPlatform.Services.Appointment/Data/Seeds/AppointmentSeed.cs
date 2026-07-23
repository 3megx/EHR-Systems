using Microsoft.EntityFrameworkCore;

namespace EHRPlatform.Services.Appointment.Data.Seeds;

/// <summary>
/// Seed data for Appointments, Reminders, and Provider Availability.
/// </summary>
public static class AppointmentSeed
{
    public static void SeedAppointments(this ModelBuilder modelBuilder)
    {
        var appointmentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var reminderId = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var providerId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Entities.Appointment>().HasData(
            new Entities.Appointment
            {
                Id = appointmentId,
                PatientId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ProviderId = providerId,
                ScheduledStart = DateTime.UtcNow.AddDays(1),
                ScheduledEnd = DateTime.UtcNow.AddDays(1).AddHours(1),
                Status = "Scheduled",
                AppointmentType = "Office",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<AppointmentReminder>().HasData(
            new AppointmentReminder
            {
                Id = reminderId,
                AppointmentId = appointmentId,
                ReminderType = "Email",
                ScheduledTime = DateTime.UtcNow.AddDays(1).AddHours(-1),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<ProviderAvailability>().HasData(
            new ProviderAvailability
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                ProviderId = providerId,
                AvailableStart = DateTime.UtcNow.AddDays(1),
                AvailableEnd = DateTime.UtcNow.AddDays(1).AddHours(8),
                Status = "Available",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
