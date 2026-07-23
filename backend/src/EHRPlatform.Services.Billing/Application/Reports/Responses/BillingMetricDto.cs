namespace EHRPlatform.Services.Billing.Application.Reports.Responses;

/// <summary>
/// Billing metric DTO.
/// Represents daily billing metrics (invoiced, paid, insurance claims).
/// </summary>
public class BillingMetricDto
{
    public DateTime Date { get; set; }
    public decimal Invoiced { get; set; }
    public decimal Paid { get; set; }
    public decimal InsuranceClaims { get; set; }
}
