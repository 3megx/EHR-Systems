using EHRPlatform.Common.CQRS;
using EHRPlatform.Services.Billing.Application.Invoicing.Responses;

namespace EHRPlatform.Services.Billing.Features.Invoicing.Commands;

/// <summary>
/// Create invoice command.
/// </summary>
public record CreateInvoiceCommand : ICommand<InvoiceResponseDto>
{
    public Guid PatientId { get; init; }
    public Guid? AppointmentId { get; init; }
    public DateTime ServiceDate { get; init; }
    public List<LineItemRequest> LineItems { get; init; } = new();
    public string? InsuranceProvider { get; init; }
    public string? InsurancePolicyNumber { get; init; }
    public string? Notes { get; init; }
}

public class LineItemRequest
{
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
