using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Appointment.Application.AppointmentManagement.Mappers;
using EHRPlatform.Services.Appointment.Application.AppointmentManagement.Responses;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Queries;

/// <summary>
/// Get appointment by ID handler.
/// Delegates all mapping to AppointmentMapper (SRP).
/// </summary>
public class GetAppointmentQueryHandler : IQueryHandler<GetAppointmentQuery, AppointmentResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppointmentMapper _mapper;
    private readonly ILogger<GetAppointmentQueryHandler> _logger;

    public GetAppointmentQueryHandler(
        IUnitOfWork unitOfWork,
        AppointmentMapper mapper,
        ILogger<GetAppointmentQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AppointmentResponseDto> Handle(
        GetAppointmentQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching appointment {AppointmentId}", request.AppointmentId);

        var repo = _unitOfWork.Repository<Domain.Appointment>();
        var appointment = await repo.FirstOrDefaultAsync(
            q => q.Where(a => a.Id == request.AppointmentId),
            cancellationToken);

        if (appointment == null)
            throw new InvalidOperationException($"Appointment {request.AppointmentId} not found");

        return _mapper.MapToResponseDto(appointment);
    }
}

/// <summary>
/// Get patient appointments handler.
/// Delegates pagination mapping to AppointmentMapper (SRP).
/// </summary>
public class GetPatientAppointmentsQueryHandler : IQueryHandler<GetPatientAppointmentsQuery, AppointmentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppointmentMapper _mapper;
    private readonly ILogger<GetPatientAppointmentsQueryHandler> _logger;

    public GetPatientAppointmentsQueryHandler(
        IUnitOfWork unitOfWork,
        AppointmentMapper mapper,
        ILogger<GetPatientAppointmentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AppointmentListDto> Handle(
        GetPatientAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching appointments for patient {PatientId}", request.PatientId);

        var repo = _unitOfWork.Repository<Domain.Appointment>();
        var skip = (request.PageNumber - 1) * request.PageSize;

        var total = await repo.CountAsync(
            q => q.Where(a => a.PatientId == request.PatientId),
            cancellationToken);

        var appointments = await repo.ToListAsync(
            q => q.Where(a => a.PatientId == request.PatientId)
                .Where(a => a.ScheduledStart >= (request.FromDate ?? DateTime.MinValue))
                .Where(a => a.ScheduledStart <= (request.ToDate ?? DateTime.MaxValue))
                .OrderByDescending(a => a.ScheduledStart)
                .Skip(skip)
                .Take(request.PageSize),
            cancellationToken);

        return _mapper.MapToListDto(appointments, total, request.PageNumber, request.PageSize);
    }
}

/// <summary>
/// Get provider appointments calendar handler.
/// Delegates calendar and slot mapping to AppointmentMapper (SRP).
/// Eliminates inline slot DTO creation.
/// </summary>
public class GetProviderAppointmentsQueryHandler : IQueryHandler<GetProviderAppointmentsQuery, ProviderAppointmentCalendarDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppointmentMapper _mapper;
    private readonly ILogger<GetProviderAppointmentsQueryHandler> _logger;

    public GetProviderAppointmentsQueryHandler(
        IUnitOfWork unitOfWork,
        AppointmentMapper mapper,
        ILogger<GetProviderAppointmentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProviderAppointmentCalendarDto> Handle(
        GetProviderAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching calendar for provider {ProviderId} on {Date:yyyy-MM-dd}", request.ProviderId, request.Date);

        var dayStart = request.Date.Date;
        var dayEnd = dayStart.AddDays(1);

        var appointmentRepo = _unitOfWork.Repository<Domain.Appointment>();
        var appointments = await appointmentRepo.ToListAsync(
            q => q.Where(a =>
                a.ProviderId == request.ProviderId &&
                a.ScheduledStart >= dayStart &&
                a.ScheduledStart < dayEnd)
                .OrderBy(a => a.ScheduledStart),
            cancellationToken);

        return _mapper.MapToProviderCalendarDto(request.ProviderId, request.Date, appointments);
    }
}

/// <summary>
/// Get provider availability slots handler.
/// Delegates availability mapping to AppointmentMapper (SRP).
/// Eliminates inline ProviderAvailabilitySlotDto creation.
/// </summary>
public class GetProviderAvailabilityQueryHandler : IQueryHandler<GetProviderAvailabilityQuery, ProviderAvailabilityListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppointmentMapper _mapper;
    private readonly ILogger<GetProviderAvailabilityQueryHandler> _logger;

    public GetProviderAvailabilityQueryHandler(
        IUnitOfWork unitOfWork,
        AppointmentMapper mapper,
        ILogger<GetProviderAvailabilityQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProviderAvailabilityListDto> Handle(
        GetProviderAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Fetching availability for provider {ProviderId} from {From} to {To}",
            request.ProviderId, request.FromDate, request.ToDate);

        var repo = _unitOfWork.Repository<ProviderAvailability>();
        var slots = await repo.ToListAsync(
            q => q.Where(a =>
                a.ProviderId == request.ProviderId &&
                a.IsActive &&
                a.SlotStart >= request.FromDate &&
                a.SlotEnd <= request.ToDate)
                .OrderBy(a => a.SlotStart),
            cancellationToken);

        return _mapper.MapToAvailabilityListDto(request.ProviderId, slots);
    }
}
