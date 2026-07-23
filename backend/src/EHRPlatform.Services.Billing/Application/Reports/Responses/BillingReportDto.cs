namespace EHRPlatform.Services.Billing.Application.Reports.Responses;

/// <summary>
/// Billing report DTO.
/// Contains aggregate billing metrics and comprehensive financial information.
/// </summary>
public class BillingReportDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalOutstanding { get; set; }
    public decimal TotalInsuranceClaims { get; set; }
    public int InvoiceCount { get; set; }
    public int PatientCount { get; set; }
    public double CollectionRate { get; set; }
    public List<BillingMetricDto> DailyMetrics { get; set; } = new();
}

