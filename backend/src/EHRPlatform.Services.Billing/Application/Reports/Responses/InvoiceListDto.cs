namespace EHRPlatform.Services.Billing.Application.Reports.Responses;

/// <summary>
/// Invoice list DTO with pagination.
/// </summary>
public class InvoiceListDto
{
    public List<InvoiceResponseDto> Items { get; set; } = new();
    public int Total { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
