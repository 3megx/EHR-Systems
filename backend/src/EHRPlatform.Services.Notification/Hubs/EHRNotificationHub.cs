using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Services.Notification.Hubs;

/// <summary>
/// SignalR hub for real-time EHR notifications pushed to the Angular frontend.
///
/// Connection flow:
///   1. Angular client connects: hub.start()
///   2. Client joins a tenant group: hub.joinTenant(tenantId)
///   3. Server pushes events to the group (e.g. new lab result, vital alert)
///
/// Groups:
///   tenant:{tenantId}           – all users in a tenant
///   patient:{patientId}         – clinicians viewing a specific patient
///   doctor:{userId}             – notifications for a specific doctor
///
/// HIPAA: only opaque IDs (PatientId, TenantId) are sent through SignalR.
/// PII (name, DOB, diagnosis) must NOT appear in real-time messages.
/// </summary>
public sealed class EHRNotificationHub : Hub
{
    private readonly ILogger<EHRNotificationHub> _logger;

    public EHRNotificationHub(ILogger<EHRNotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>Client joins the tenant-wide notification group.</summary>
    public async Task JoinTenant(string tenantId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant:{tenantId}");
        _logger.LogDebug("Connection {ConnectionId} joined tenant group {TenantId}",
            Context.ConnectionId, tenantId);
    }

    /// <summary>Client subscribes to real-time updates for a specific patient.</summary>
    public async Task JoinPatientRoom(string patientId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"patient:{patientId}");
        _logger.LogDebug("Connection {ConnectionId} joined patient room {PatientId}",
            Context.ConnectionId, patientId);
    }

    /// <summary>Client leaves the patient room (e.g. navigating away).</summary>
    public async Task LeavePatientRoom(string patientId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patient:{patientId}");
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("SignalR client connected: {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("SignalR client disconnected: {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }
}
