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
    private readonly ReportMapper _mapper;
    private readonly ILogger<GetPatientOutstandingBalanceQueryHandler> _logger;

    public GetPatientOutstandingBalanceQueryHandler(
        IUnitOfWork unitOfWork,
        ReportMapper mapper,
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
    private readonly ReportMapper _mapper;
    private readonly ILogger<GetBillingReportQueryHandler> _logger;

    public GetBillingReportQueryHandler(
        IUnitOfWork unitOfWork,
        ReportMapper mapper,
        ILogger<GetBillingReportQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        // Delegate mapping and metrics calculation to mapper
        var report = _mapper.MapToBillingReportDto(request.StartDate, request.EndDate, invoices);

        _logger.LogInformation("Billing report generated with {InvoiceCount} invoices", invoices.Count);

        return report;
    }
}
