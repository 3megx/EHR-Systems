using Microsoft.EntityFrameworkCore;

namespace EHRPlatform.Services.Appointment.Data.Seeds;

/// <summary>
/// Seed data for Appointments.
/// </summary>
public static class AppointmentSeed
{
    public static void SeedAppointments(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Entities.Appointment>().HasData(
            new Entities.Appointment
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                PatientId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                ProviderId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                ScheduledStart = DateTime.UtcNow.AddDays(1),
                ScheduledEnd = DateTime.UtcNow.AddDays(1).AddHours(1),
                Status = "Scheduled",
                AppointmentType = "Office",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
