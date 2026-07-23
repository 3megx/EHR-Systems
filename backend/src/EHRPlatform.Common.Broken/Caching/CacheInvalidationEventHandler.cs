using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace EHRPlatform.Common.Caching;

/// <summary>
/// Handles cache invalidation based on domain events.
/// Subscribed to integration events from Kafka to invalidate cache
/// when data changes in other services.
/// 
/// Pattern: Event-Driven Cache Invalidation
/// 1. Service publishes IntegrationEvent to Kafka
/// 2. This handler subscribes and receives event
/// 3. Maps event to cache invalidation patterns
/// 4. Removes affected cache entries
/// 5. Ensures cache consistency across distributed system
/// </summary>
public class CacheInvalidationEventHandler
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheInvalidationEventHandler> _logger;

    public CacheInvalidationEventHandler(
        ICacheService cacheService,
        ILogger<CacheInvalidationEventHandler> logger)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handle patient-related events.
    /// Invalidates all patient caches when patient data changes.
    /// </summary>
    public async Task HandlePatientEventAsync(
        string eventType,
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        var patterns = eventType switch
        {
            "PatientCreated" => new[] { CacheKeyGenerator.PatientsPatternKey() },
            "PatientUpdated" => new[] 
            { 
                CacheKeyGenerator.PatientKey(patientId),
                CacheKeyGenerator.PatientPatternKey(patientId),
                CacheKeyGenerator.PatientsListKey(),
                CacheKeyGenerator.PatientsPatternKey()
            },
            "PatientDeleted" => new[]
            {
                CacheKeyGenerator.PatientKey(patientId),
                CacheKeyGenerator.PatientPatternKey(patientId),
                CacheKeyGenerator.PatientsListKey(),
                CacheKeyGenerator.PatientsPatternKey()
            },
            _ => Array.Empty<string>()
        };

        await InvalidatePatternsAsync(patterns, $"patient_{patientId}_{eventType}", cancellationToken);
    }

    /// <summary>
    /// Handle appointment-related events.
    /// </summary>
    public async Task HandleAppointmentEventAsync(
        string eventType,
        Guid appointmentId,
        Guid? patientId = null,
        Guid? doctorId = null,
        CancellationToken cancellationToken = default)
    {
        var patterns = new List<string> { CacheKeyGenerator.AppointmentsPatternKey() };

        if (patientId.HasValue)
            patterns.Add(CacheKeyGenerator.AppointmentsByPatientKey(patientId.Value));

        if (doctorId.HasValue)
            patterns.Add(CacheKeyGenerator.AppointmentsByDoctorKey(doctorId.Value, DateTime.Now));

        await InvalidatePatternsAsync(
            patterns.ToArray(),
            $"appointment_{appointmentId}_{eventType}",
            cancellationToken);
    }

    /// <summary>
    /// Handle clinical data events (SOAP notes, vital signs, etc.).
    /// </summary>
    public async Task HandleClinicalEventAsync(
        string eventType,
        Guid patientId,
        string clinicalDataType,
        CancellationToken cancellationToken = default)
    {
        var patterns = clinicalDataType switch
        {
            "SoapNote" => new[]
            {
                CacheKeyGenerator.PatientSoapNotesKey(patientId),
                CacheKeyGenerator.PatientClinicalPatternKey(patientId)
            },
            "VitalSigns" => new[]
            {
                CacheKeyGenerator.PatientVitalsKey(patientId),
                CacheKeyGenerator.PatientClinicalPatternKey(patientId)
            },
            "Diagnosis" => new[]
            {
                CacheKeyGenerator.PatientDiagnosesKey(patientId),
                CacheKeyGenerator.PatientClinicalPatternKey(patientId)
            },
            "Allergy" => new[]
            {
                CacheKeyGenerator.PatientAllergiesKey(patientId),
                CacheKeyGenerator.PatientClinicalPatternKey(patientId)
            },
            "Condition" => new[]
            {
                CacheKeyGenerator.PatientConditionsKey(patientId),
                CacheKeyGenerator.PatientClinicalPatternKey(patientId)
            },
            _ => new[] { CacheKeyGenerator.PatientClinicalPatternKey(patientId) }
        };

        await InvalidatePatternsAsync(
            patterns,
            $"clinical_{clinicalDataType}_{patientId}_{eventType}",
            cancellationToken);
    }

    /// <summary>
    /// Handle reference data updates (codes, lookup tables, etc.).
    /// Invalidates all reference data caches.
    /// </summary>
    public async Task HandleReferenceDataEventAsync(
        string eventType,
        string dataType,
        CancellationToken cancellationToken = default)
    {
        var patterns = new[]
        {
            CacheKeyGenerator.ReferenceDataPatternKey(),
            CacheKeyGenerator.MedicalCodesSearchKey(dataType, "*")
        };

        await InvalidatePatternsAsync(
            patterns,
            $"referencedata_{dataType}_{eventType}",
            cancellationToken);
    }

    /// <summary>
    /// Handle user/authentication events.
    /// Invalidates user caches and permissions.
    /// </summary>
    public async Task HandleUserEventAsync(
        string eventType,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var patterns = eventType switch
        {
            "UserUpdated" => new[]
            {
                CacheKeyGenerator.UserKey(userId),
                CacheKeyGenerator.UserRolesKey(userId),
                CacheKeyGenerator.UserPermissionsKey(userId)
            },
            "UserDeleted" => new[]
            {
                CacheKeyGenerator.UserKey(userId),
                CacheKeyGenerator.UserRolesKey(userId),
                CacheKeyGenerator.UserPermissionsKey(userId)
            },
            "RolesAssigned" => new[]
            {
                CacheKeyGenerator.UserRolesKey(userId),
                CacheKeyGenerator.UserPermissionsKey(userId)
            },
            "PermissionsUpdated" => new[]
            {
                CacheKeyGenerator.UserPermissionsKey(userId)
            },
            _ => Array.Empty<string>()
        };

        await InvalidatePatternsAsync(
            patterns,
            $"user_{userId}_{eventType}",
            cancellationToken);
    }

    /// <summary>
    /// Generic invalidation handler for custom events.
    /// </summary>
    public async Task HandleGenericEventAsync(
        IEnumerable<string> patternsToInvalidate,
        string eventIdentifier,
        CancellationToken cancellationToken = default)
    {
        await InvalidatePatternsAsync(
            patternsToInvalidate.ToArray(),
            eventIdentifier,
            cancellationToken);
    }

    /// <summary>
    /// Internal method to execute pattern-based invalidations.
    /// </summary>
    private async Task InvalidatePatternsAsync(
        string[] patterns,
        string eventIdentifier,
        CancellationToken cancellationToken = default)
    {
        if (patterns.Length == 0)
        {
            _logger.LogDebug("No cache patterns to invalidate for event: {EventId}", eventIdentifier);
            return;
        }

        try
        {
            var totalRemoved = 0L;

            foreach (var pattern in patterns)
            {
                try
                {
                    var removed = await _cacheService.RemoveByPatternAsync(pattern, cancellationToken);
                    totalRemoved += removed;
                    _logger.LogDebug(
                        "Invalidated {Count} cache entries for pattern {Pattern} (Event: {EventId})",
                        removed,
                        pattern,
                        eventIdentifier);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Failed to invalidate cache pattern {Pattern} for event {EventId}",
                        pattern,
                        eventIdentifier);
                    // Continue with next pattern
                }
            }

            _logger.LogInformation(
                "Cache invalidation completed for event {EventId}. Total entries removed: {Count}",
                eventIdentifier,
                totalRemoved);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unexpected error during cache invalidation for event {EventId}",
                eventIdentifier);
            // Don't throw - cache invalidation failures shouldn't break the application
        }
    }
}

/// <summary>
/// Extensions for cache invalidation in MediatR handlers.
/// Use in command handlers after data mutations.
/// </summary>
public static class CacheInvalidationExtensions
{
    /// <summary>
    /// Create an invalidation builder from a cache service.
    /// Fluent API for building cache invalidation chains.
    /// </summary>
    public static CacheInvalidationBuilder CreateInvalidation(this ICacheService cacheService)
    {
        return new CacheInvalidationBuilder(cacheService);
    }

    /// <summary>
    /// Invalidate all patient-related caches.
    /// </summary>
    public static async Task InvalidatePatientCacheAsync(
        this ICacheService cacheService,
        Guid patientId,
        bool invalidateAllPatients = false,
        CancellationToken cancellationToken = default)
    {
        var patterns = invalidateAllPatients
            ? new[] { CacheKeyGenerator.PatientsPatternKey() }
            : new[] { CacheKeyGenerator.PatientPatternKey(patientId) };

        foreach (var pattern in patterns)
        {
            await cacheService.RemoveByPatternAsync(pattern, cancellationToken);
        }
    }

    /// <summary>
    /// Invalidate specific patient cache entries.
    /// </summary>
    public static async Task InvalidatePatientSpecificAsync(
        this ICacheService cacheService,
        Guid patientId,
        string dataType = "all",
        CancellationToken cancellationToken = default)
    {
        var keysToRemove = dataType switch
        {
            "demographics" => new[] { CacheKeyGenerator.PatientKey(patientId) },
            "allergies" => new[] { CacheKeyGenerator.PatientAllergiesKey(patientId) },
            "conditions" => new[] { CacheKeyGenerator.PatientConditionsKey(patientId) },
            "vitals" => new[] { CacheKeyGenerator.PatientVitalsKey(patientId) },
            "clinical" => new[] { CacheKeyGenerator.PatientClinicalPatternKey(patientId) },
            _ => new[] { CacheKeyGenerator.PatientPatternKey(patientId) }
        };

        await cacheService.RemoveAsync(keysToRemove, cancellationToken);
    }

    /// <summary>
    /// Invalidate search result caches.
    /// </summary>
    public static async Task InvalidateSearchCacheAsync(
        this ICacheService cacheService,
        string searchType = "patients",
        CancellationToken cancellationToken = default)
    {
        var pattern = $"{searchType}:search:*";
        await cacheService.RemoveByPatternAsync(pattern, cancellationToken);
    }
}
