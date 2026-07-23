using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Billing.Features.Reports.Queries;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Billing.Features.Reports.Handlers;

/// <summary>
/// Get patient invoices handler (for reporting).
/// Pure business logic - no mapping responsibility.
/// </summary>
public class GetPatientInvoicesQueryHandler : IQueryHandler<GetPatientInvoicesQuery, InvoiceListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly InvoiceMapper _mapper;
    private readonly ILogger<GetPatientInvoicesQueryHandler> _logger;

    public GetPatientInvoicesQueryHandler(
        IUnitOfWork unitOfWork,
        InvoiceMapper mapper,
        ILogger<GetPatientInvoicesQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        // Delegate mapping to mapper
        return _mapper.MapToListDto(invoices, total, request.PageNumber, request.PageSize);
    }
}

/// <summary>
/// Get outstanding balance handler (for reporting).
/// Pure business logic - no mapping responsibility.
/// </summary>
public class GetPatientOutstandingBalanceQueryHandler : IQueryHandler<GetPatientOutstandingBalanceQuery, OutstandingBalanceDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly InvoiceMapper _mapper;
    private readonly ILogger<GetPatientOutstandingBalanceQueryHandler> _logger;

    public GetPatientOutstandingBalanceQueryHandler(
        IUnitOfWork unitOfWork,
        InvoiceMapper mapper,
        ILogger<GetPatientOutstandingBalanceQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        // Delegate mapping and balance calculation to mapper
        return _mapper.MapToOutstandingBalanceDto(request.PatientId, invoices);
    }
}

/// <summary>
/// Get billing report handler.
/// Pure business logic - generates aggregate billing metrics.
/// </summary>
public class GetBillingReportQueryHandler : IQueryHandler<GetBillingReportQuery, BillingReportDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetBillingReportQueryHandler> _logger;

    public GetBillingReportQueryHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetBillingReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BillingReportDto> Handle(
        GetBillingReportQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating billing report for {StartDate} to {EndDate}",
            request.StartDate, request.EndDate);

        var repo = _unitOfWork.Repository<Invoice>();
        var invoices = await repo.ToListAsync(
            q => q.Where(i => i.ServiceDate >= request.StartDate && i.ServiceDate <= request.EndDate),
            cancellationToken);

        var totalInvoiced = invoices.Sum(i => i.TotalAmount);
        var totalPaid = invoices.Sum(i => i.AmountPaid);
        var totalOutstanding = invoices.Sum(i => i.BalanceDue);
        var collectionRate = totalInvoiced > 0 ? (double)(totalPaid / totalInvoiced) : 0;

        var dailyMetrics = invoices
            .GroupBy(i => i.ServiceDate.Date)
            .Select(g => new BillingMetricDto
            {
                Date = g.Key,
                Invoiced = g.Sum(i => i.TotalAmount),
                Paid = g.Sum(i => i.AmountPaid),
                InsuranceClaims = g.Sum(i => i.TotalAmount) // placeholder
            })
            .OrderBy(m => m.Date)
            .ToList();

        var report = new BillingReportDto
        {
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalInvoiced = totalInvoiced,
            TotalPaid = totalPaid,
            TotalOutstanding = totalOutstanding,
            TotalInsuranceClaims = 0, // calculate from claims data
            InvoiceCount = invoices.Count,
            PatientCount = invoices.Select(i => i.PatientId).Distinct().Count(),
            CollectionRate = collectionRate,
            DailyMetrics = dailyMetrics
        };

        _logger.LogInformation("Billing report generated with {InvoiceCount} invoices", invoices.Count);

        return report;
    }
}
