using Microsoft.EntityFrameworkCore;
using EHRPlatform.Services.Prescription.Domain.Entities;

namespace EHRPlatform.Services.Prescription.Data.Seeds;

/// <summary>
/// Seed data for Prescription (Prescriptions and Refills).
/// </summary>
public static class PrescriptionSeed
{
    public static void SeedPrescriptions(this ModelBuilder modelBuilder)
    {
        var prescriptionId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var patientId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var providerId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Prescription>().HasData(
            new Prescription
            {
                Id = prescriptionId,
                PatientId = patientId,
                ProviderId = providerId,
                MedicationName = "Amoxicillin",
                Dosage = "500mg",
                Frequency = "Twice daily",
                Quantity = 20,
                RefillsRemaining = 3,
                IssuedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            }
        );

        modelBuilder.Entity<PrescriptionRefill>().HasData(
            new PrescriptionRefill
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                PrescriptionId = prescriptionId,
                RefillDate = DateTime.UtcNow,
                Quantity = 20,
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
