using Mapster;
using EHRPlatform.Common.Mapping;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Billing.Application.Invoicing;

/// <summary>
/// Invoice Mapper
/// Single Responsibility: Convert between Invoice domain models and DTOs.
/// Handles all Invoice-related mappings with optional post-processing.
/// </summary>
public class InvoiceMapper : MappingServiceBase<Invoice, InvoiceResponseDto>
{
    public InvoiceMapper(ILogger<InvoiceMapper> logger) : base(logger)
    {
    }

    /// <summary>
    /// Map single invoice to response DTO.
    /// </summary>
    public InvoiceResponseDto MapToResponseDto(Invoice invoice)
    {
        return MapToDto(invoice);
    }

    /// <summary>
    /// Map collection of invoices to paginated DTO.
    /// </summary>
    public InvoiceListDto MapToListDto(
        ICollection<Invoice> invoices,
        int total,
        int pageNumber,
        int pageSize)
    {
        Logger.LogDebug("Mapping {Count} invoices to paginated list DTO", invoices.Count);

        return new InvoiceListDto
        {
            Items = invoices.Adapt<List<InvoiceResponseDto>>(),
            Total = total,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Map collection of invoices to response DTO list.
    /// </summary>
    public List<InvoiceResponseDto> MapToResponseDtoList(ICollection<Invoice> invoices)
    {
        Logger.LogDebug("Mapping {Count} invoices to response DTO list", invoices.Count);
        return invoices.Adapt<List<InvoiceResponseDto>>();
    }

    /// <summary>
    /// Map invoices with enriched balance and aging information.
    /// </summary>
    public OutstandingBalanceDto MapToOutstandingBalanceDto(
        Guid patientId,
        ICollection<Invoice> invoices)
    {
        Logger.LogDebug("Mapping outstanding balance for patient {PatientId}", patientId);

        var invoiceDtos = invoices.Adapt<List<InvoiceResponseDto>>();
        var totalBalance = invoices.Sum(i => i.BalanceDue);
        var overdueInvoices = invoices.Count(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid");
        var overdueAmount = invoices
            .Where(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid")
            .Sum(i => i.BalanceDue);

        return new OutstandingBalanceDto
        {
            PatientId = patientId,
            TotalBalance = totalBalance,
            OverdueInvoices = overdueInvoices,
            OverdueAmount = overdueAmount,
            Invoices = invoiceDtos
        };
    }

    /// <summary>
    /// Map invoice to command DTO.
    /// </summary>
    public InvoiceCommandDto MapToCommandDto(Invoice invoice)
    {
        Logger.LogDebug("Mapping invoice {InvoiceId} to command DTO", invoice.Id);

        return new InvoiceCommandDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            PatientId = invoice.PatientId,
            AppointmentId = invoice.AppointmentId,
            ServiceDate = invoice.ServiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            SubTotal = invoice.SubTotal,
            TaxAmount = invoice.TaxAmount,
            TotalAmount = invoice.TotalAmount,
            InsuranceProvider = invoice.InsuranceProvider,
            InsurancePolicyNumber = invoice.InsurancePolicyNumber,
            Notes = invoice.Notes
        };
    }
}
