using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Billing.Features.Billing.Domain;
using EHRPlatform.Services.Billing.Features.Billing.Dtos.Responses;
using EHRPlatform.Services.Billing.Mappings;

namespace EHRPlatform.Services.Billing.Features.Billing.Queries;

/// <summary>
/// Get invoice by ID handler.
/// Pure business logic - no mapping responsibility.
/// </summary>
public class GetInvoiceQueryHandler : IQueryHandler<GetInvoiceQuery, InvoiceResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly InvoiceMapper _mapper;
    private readonly ILogger<GetInvoiceQueryHandler> _logger;

    public GetInvoiceQueryHandler(
        IUnitOfWork unitOfWork,
        InvoiceMapper mapper,
        ILogger<GetInvoiceQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
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

        // Delegate mapping to mapper - clean separation of concerns
        return _mapper.MapToResponseDto(invoice);
    }
}

/// <summary>
/// Get patient invoices handler.
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
/// Get outstanding balance handler.
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
