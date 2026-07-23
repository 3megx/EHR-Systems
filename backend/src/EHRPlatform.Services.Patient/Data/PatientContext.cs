using Microsoft.EntityFrameworkCore;
using EHRPlatform.Common.Data;

namespace EHRPlatform.Services.Patient.Data;

/// <summary>
/// DbContext for Patient Service.
/// Manages patients, allergies, conditions.
/// </summary>
public class PatientContext : BaseDbContext
{
    public PatientContext(DbContextOptions<PatientContext> options) : base(options) { }

    public DbSet<Entities.Patient> Patients { get; set; } = null!;
    public DbSet<PatientAllergy> PatientAllergies { get; set; } = null!;
    public DbSet<PatientCondition> PatientConditions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new Configuration.PatientConfiguration());
        modelBuilder.ApplyConfiguration(new Configuration.PatientAllergyConfiguration());
        modelBuilder.ApplyConfiguration(new Configuration.PatientConditionConfiguration());

        // Apply seeds
        modelBuilder.SeedPatients();
    }
}
