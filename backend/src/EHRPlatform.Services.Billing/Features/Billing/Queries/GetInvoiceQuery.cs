using EHRPlatform.Common.CQRS;

namespace EHRPlatform.Services.Billing.Features.Billing.Queries;

/// <summary>
/// Get invoice by ID - CACHED query.
/// </summary>
public record GetInvoiceQuery : ICachedQuery<InvoiceResponseDto>
{
    public Guid InvoiceId { get; init; }

    public string CacheKey => $"invoice_{InvoiceId}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get patient invoices - CACHED query.
/// </summary>
public record GetPatientInvoicesQuery : ICachedQuery<InvoiceListDto>
{
    public Guid PatientId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;

    public string CacheKey => $"invoices_patient_{PatientId}_{PageNumber}_{PageSize}";
    public int CacheDurationSeconds => 600;
}

/// <summary>
/// Get outstanding balance - CACHED query.
/// </summary>
public record GetPatientOutstandingBalanceQuery : ICachedQuery<OutstandingBalanceDto>
{
    public Guid PatientId { get; init; }

    public string CacheKey => $"balance_patient_{PatientId}";
    public int CacheDurationSeconds => 300;
}

/// <summary>
/// Invoice list DTO.
/// </summary>
public class InvoiceListDto
{
    public List<InvoiceResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

/// <summary>
/// Invoice response DTO.
/// </summary>
public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal BalanceDue { get; set; }
    public string? InsuranceProvider { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<LineItemDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
    public List<ClaimDto> Claims { get; set; } = new();
}

public class LineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CPTCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}

public class ClaimDto
{
    public Guid Id { get; set; }
    public string InsuranceProvider { get; set; } = string.Empty;
    public string ClaimNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

/// <summary>
/// Outstanding balance DTO.
/// </summary>
public class OutstandingBalanceDto
{
    public Guid PatientId { get; set; }
    public decimal TotalBalance { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal OverdueAmount { get; set; }
    public List<InvoiceResponseDto> Invoices { get; set; } = new();
}
