using EHRPlatform.Common.CQRS;
using EHRPlatform.Common.Data;
using EHRPlatform.Services.Appointment.Features.Appointments.Domain;
using Mapster;

namespace EHRPlatform.Services.Appointment.Features.Appointments.Queries;

/// <summary>
/// Get appointment by ID handler.
/// </summary>
public class GetAppointmentQueryHandler : IQueryHandler<GetAppointmentQuery, AppointmentResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetAppointmentQueryHandler> _logger;

    public GetAppointmentQueryHandler(IUnitOfWork unitOfWork, ILogger<GetAppointmentQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
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

        return appointment.Adapt<AppointmentResponseDto>();
    }
}

/// <summary>
/// Get patient appointments handler.
/// </summary>
public class GetPatientAppointmentsQueryHandler : IQueryHandler<GetPatientAppointmentsQuery, AppointmentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPatientAppointmentsQueryHandler> _logger;

    public GetPatientAppointmentsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPatientAppointmentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AppointmentListDto> Handle(
        GetPatientAppointmentsQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching appointments for patient {PatientId}", request.PatientId);

        var repo = _unitOfWork.Repository<Domain.Appointment>();
        var skip = (request.PageNumber - 1) * request.PageSize;

        var query = repo.Query()
            .Where(a => a.PatientId == request.PatientId)
            .Where(a => a.ScheduledStart >= (request.FromDate ?? DateTime.MinValue))
            .Where(a => a.ScheduledStart <= (request.ToDate ?? DateTime.MaxValue))
            .OrderByDescending(a => a.ScheduledStart);

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

        return new AppointmentListDto
        {
            Items = appointments.Adapt<List<AppointmentResponseDto>>(),
            Total = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

/// <summary>
/// Get provider appointments calendar handler.
/// </summary>
public class GetProviderAppointmentsQueryHandler : IQueryHandler<GetProviderAppointmentsQuery, ProviderAppointmentCalendarDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProviderAppointmentsQueryHandler> _logger;

    public GetProviderAppointmentsQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProviderAppointmentsQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
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

        var slots = appointments.Select(a => new AppointmentSlotDto
        {
            Start = a.ScheduledStart,
            End = a.ScheduledEnd,
            Status = a.Status == "Cancelled" ? "Available" : (a.Status == "Scheduled" || a.Status == "Confirmed" ? "Booked" : "Blocked"),
            AppointmentId = a.Id,
            PatientId = a.PatientId
        }).ToList();

        return new ProviderAppointmentCalendarDto
        {
            ProviderId = request.ProviderId,
            Date = request.Date,
            Slots = slots
        };
    }
}

/// <summary>
/// Get provider availability slots handler.
/// </summary>
public class GetProviderAvailabilityQueryHandler : IQueryHandler<GetProviderAvailabilityQuery, ProviderAvailabilityListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetProviderAvailabilityQueryHandler> _logger;

    public GetProviderAvailabilityQueryHandler(IUnitOfWork unitOfWork, ILogger<GetProviderAvailabilityQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
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

        return new ProviderAvailabilityListDto
        {
            ProviderId = request.ProviderId,
            Slots = slots.Select(s => new ProviderAvailabilitySlotDto
            {
                Id = s.Id,
                SlotStart = s.SlotStart,
                SlotEnd = s.SlotEnd,
                IsRecurring = s.IsRecurring,
                RecurrencePattern = s.RecurrencePattern,
                MaxAppointmentsPerSlot = s.MaxAppointmentsPerSlot,
                CurrentBookings = s.CurrentBookings,
                HasAvailability = s.HasAvailability()
            }).ToList()
        };
    }
}
