using Microsoft.EntityFrameworkCore;

namespace EHRPlatform.Services.Billing.Data.Seeds;

/// <summary>
/// Seed data for Reports (read-only aggregations, no direct seeding needed).
/// Reports are computed from Invoice, Payment, and InsuranceClaim data.
/// </summary>
public static class ReportSeed
{
    public static void SeedReports(this ModelBuilder modelBuilder)
    {
        // Reports are derived from Invoice, Payment, and InsuranceClaim aggregates.
        // No direct seed data required.
    }
}
