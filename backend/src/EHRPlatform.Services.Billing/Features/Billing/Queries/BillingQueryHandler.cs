using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Billing.Features.Billing.Domain;
using Mapster;

namespace EHRPlatform.Services.Billing.Features.Billing.Queries;

/// <summary>
/// Get invoice by ID handler.
/// </summary>
public class GetInvoiceQueryHandler : IQueryHandler<GetInvoiceQuery, InvoiceResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetInvoiceQueryHandler> _logger;

    public GetInvoiceQueryHandler(IUnitOfWork unitOfWork, ILogger<GetInvoiceQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<InvoiceResponseDto> Handle(
        GetInvoiceQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoice {InvoiceId}", request.InvoiceId);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoice = await repo.FirstOrDefaultAsync(
            q => q.Where(i => i.Id == request.InvoiceId),
            cancellationToken);

        if (invoice == null)
            throw new InvalidOperationException($"Invoice {request.InvoiceId} not found");

        return MapToDto(invoice);
    }

    private InvoiceResponseDto MapToDto(Invoice invoice)
    {
        var dto = invoice.Adapt<InvoiceResponseDto>();
        dto.BalanceDue = invoice.BalanceDue;
        dto.LineItems = invoice.LineItems.Select(l => new LineItemDto
        {
            Id = l.Id,
            Description = l.Description,
            CPTCode = l.CPTCode,
            Quantity = l.Quantity,
            UnitPrice = l.UnitPrice,
            Amount = l.Amount
        }).ToList();
        dto.Payments = invoice.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            Amount = p.Amount,
            Method = p.Method,
            ReceivedAt = p.ReceivedAt
        }).ToList();
        dto.Claims = invoice.InsuranceClaims.Select(c => new ClaimDto
        {
            Id = c.Id,
            InsuranceProvider = c.InsuranceProvider,
            ClaimNumber = c.ClaimNumber,
            Status = c.Status,
            Amount = c.Amount
        }).ToList();
        return dto;
    }
}

/// <summary>
/// Get patient invoices handler.
/// </summary>
public class GetPatientInvoicesQueryHandler : IQueryHandler<GetPatientInvoicesQuery, InvoiceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPatientInvoicesQueryHandler> _logger;

    public GetPatientInvoicesQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPatientInvoicesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<InvoiceListDto> Handle(
        GetPatientInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching invoices for patient {PatientId}", request.PatientId);

        var repo = _unitOfWork.Repository<Invoice>();
        var skip = (request.PageNumber - 1) * request.PageSize;

        var total = await repo.CountAsync(
            q => q.Where(i => i.PatientId == request.PatientId),
            cancellationToken);

        var invoices = await repo.ToListAsync(
            q => q.Where(i => i.PatientId == request.PatientId)
                .OrderByDescending(i => i.ServiceDate)
                .Skip(skip)
                .Take(request.PageSize),
            cancellationToken);

        return new InvoiceListDto
        {
            Items = invoices.Select(i => MapToDto(i)).ToList(),
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private InvoiceResponseDto MapToDto(Invoice invoice)
    {
        var dto = invoice.Adapt<InvoiceResponseDto>();
        dto.BalanceDue = invoice.BalanceDue;
        return dto;
    }
}

/// <summary>
/// Get outstanding balance handler.
/// </summary>
public class GetPatientOutstandingBalanceQueryHandler : IQueryHandler<GetPatientOutstandingBalanceQuery, OutstandingBalanceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPatientOutstandingBalanceQueryHandler> _logger;

    public GetPatientOutstandingBalanceQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPatientOutstandingBalanceQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<OutstandingBalanceDto> Handle(
        GetPatientOutstandingBalanceQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating balance for patient {PatientId}", request.PatientId);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoices = await repo.ToListAsync(
            q => q.Where(i => i.PatientId == request.PatientId && i.Status != "Cancelled"),
            cancellationToken);

        var totalBalance = invoices.Sum(i => i.BalanceDue);
        var overdueInvoices = invoices.Count(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid");
        var overdueAmount = invoices.Where(i => i.DueDate < DateTime.UtcNow && i.Status != "Paid").Sum(i => i.BalanceDue);

        return new OutstandingBalanceDto
        {
            PatientId = request.PatientId,
            TotalBalance = totalBalance,
            OverdueInvoices = overdueInvoices,
            OverdueAmount = overdueAmount,
            Invoices = invoices.Select(i => MapToDto(i)).ToList()
        };
    }

    private InvoiceResponseDto MapToDto(Invoice invoice)
    {
        var dto = invoice.Adapt<InvoiceResponseDto>();
        dto.BalanceDue = invoice.BalanceDue;
        return dto;
    }
}
