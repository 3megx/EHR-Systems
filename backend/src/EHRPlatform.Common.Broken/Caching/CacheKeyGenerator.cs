using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHRPlatform.Common.Caching;

/// <summary>
/// Centralized cache key generation for consistent key naming across all services.
/// Follows pattern-based organization for bulk invalidation.
/// 
/// Key Format Conventions:
/// - Entity: "entity:id" (e.g., "patient:123")
/// - List: "entity:list" (e.g., "patients:list")
/// - Paged: "entity:paged:page:size" (e.g., "patients:paged:1:10")
/// - Search: "entity:search:term:page:size" (e.g., "patients:search:john:1:10")
/// - Detail: "entity:id:detail" (e.g., "patient:123:detail")
/// - Related: "entity:id:related:type" (e.g., "patient:123:allergies")
/// 
/// Patterns for invalidation:
/// - "patient:*" - all patient caches
/// - "patient:123:*" - all caches for patient 123
/// - "patients:*" - all patients-related caches
/// </summary>
public static class CacheKeyGenerator
{
    private const string KeySeparator = ":";

    #region Patient Cache Keys

    /// <summary>
    /// Cache key for single patient by ID.
    /// Invalidated on: UpdatePatient, DeletePatient
    /// </summary>
    public static string PatientKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}";
    }

    /// <summary>
    /// Cache key for all patients (non-paginated).
    /// Invalidated on: CreatePatient, UpdatePatient, DeletePatient
    /// </summary>
    public static string PatientsListKey()
    {
        return $"patients{KeySeparator}list";
    }

    /// <summary>
    /// Cache key for paginated patients list.
    /// Includes page number and size in key.
    /// Invalidated on: CreatePatient, UpdatePatient, DeletePatient
    /// </summary>
    public static string PatientsPagedKey(int pageNumber, int pageSize)
    {
        return $"patients{KeySeparator}paged{KeySeparator}{pageNumber}{KeySeparator}{pageSize}";
    }

    /// <summary>
    /// Cache key for searched patients.
    /// Includes all filter parameters.
    /// Invalidated on: CreatePatient, UpdatePatient
    /// </summary>
    public static string PatientsSearchKey(string searchTerm, int pageNumber, int pageSize)
    {
        var term = searchTerm?.Trim() ?? "";
        return $"patients{KeySeparator}search{KeySeparator}{HashString(term)}{KeySeparator}{pageNumber}{KeySeparator}{pageSize}";
    }

    /// <summary>
    /// Cache key for patient allergies.
    /// Invalidated on: AddPatientAllergy, RemovePatientAllergy
    /// </summary>
    public static string PatientAllergiesKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}allergies";
    }

    /// <summary>
    /// Cache key for patient conditions.
    /// Invalidated on: AddPatientCondition, RemovePatientCondition
    /// </summary>
    public static string PatientConditionsKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}conditions";
    }

    /// <summary>
    /// Cache key for patient medical history timeline.
    /// Invalidated on: Any clinical event for patient
    /// </summary>
    public static string PatientTimelineKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}timeline";
    }

    /// <summary>
    /// Pattern to invalidate all patient caches.
    /// Usage: await cache.RemoveByPatternAsync(PatientsPatternKey());
    /// </summary>
    public static string PatientsPatternKey()
    {
        return "patient*";
    }

    /// <summary>
    /// Pattern to invalidate all caches for specific patient.
    /// Usage: await cache.RemoveByPatternAsync(PatientPatternKey(patientId));
    /// </summary>
    public static string PatientPatternKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}*";
    }

    #endregion

    #region Appointment Cache Keys

    /// <summary>
    /// Cache key for single appointment by ID.
    /// </summary>
    public static string AppointmentKey(Guid appointmentId)
    {
        return $"appointment{KeySeparator}{appointmentId}";
    }

    /// <summary>
    /// Cache key for appointments by patient.
    /// </summary>
    public static string AppointmentsByPatientKey(Guid patientId, int? pageNumber = null, int? pageSize = null)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
            return $"appointments{KeySeparator}patient{KeySeparator}{patientId}{KeySeparator}paged{KeySeparator}{pageNumber}{KeySeparator}{pageSize}";
        return $"appointments{KeySeparator}patient{KeySeparator}{patientId}";
    }

    /// <summary>
    /// Cache key for appointments by doctor.
    /// </summary>
    public static string AppointmentsByDoctorKey(Guid doctorId, DateTime date)
    {
        return $"appointments{KeySeparator}doctor{KeySeparator}{doctorId}{KeySeparator}{date:yyyy-MM-dd}";
    }

    /// <summary>
    /// Pattern to invalidate all appointment caches.
    /// </summary>
    public static string AppointmentsPatternKey()
    {
        return "appointment*";
    }

    #endregion

    #region Clinical Cache Keys

    /// <summary>
    /// Cache key for SOAP note.
    /// </summary>
    public static string SoapNoteKey(Guid noteId)
    {
        return $"soapnote{KeySeparator}{noteId}";
    }

    /// <summary>
    /// Cache key for patient SOAP notes (list).
    /// </summary>
    public static string PatientSoapNotesKey(Guid patientId, int? pageNumber = null, int? pageSize = null)
    {
        if (pageNumber.HasValue && pageSize.HasValue)
            return $"patient{KeySeparator}{patientId}{KeySeparator}soapnotes{KeySeparator}{pageNumber}{KeySeparator}{pageSize}";
        return $"patient{KeySeparator}{patientId}{KeySeparator}soapnotes";
    }

    /// <summary>
    /// Cache key for patient vital signs.
    /// </summary>
    public static string PatientVitalsKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}vitals";
    }

    /// <summary>
    /// Cache key for patient diagnoses.
    /// </summary>
    public static string PatientDiagnosesKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}diagnoses";
    }

    /// <summary>
    /// Pattern to invalidate all clinical caches for patient.
    /// </summary>
    public static string PatientClinicalPatternKey(Guid patientId)
    {
        return $"patient{KeySeparator}{patientId}{KeySeparator}*";
    }

    #endregion

    #region Lookup/Reference Cache Keys

    /// <summary>
    /// Cache key for reference data (ICD-10 codes, CPT codes, etc.).
    /// </summary>
    public static string ReferenceDataKey(string dataType)
    {
        return $"ref{KeySeparator}{dataType}";
    }

    /// <summary>
    /// Cache key for medical codes search results.
    /// </summary>
    public static string MedicalCodesSearchKey(string codeType, string searchTerm)
    {
        return $"codes{KeySeparator}{codeType}{KeySeparator}{HashString(searchTerm)}";
    }

    /// <summary>
    /// Pattern to invalidate all reference data caches.
    /// </summary>
    public static string ReferenceDataPatternKey()
    {
        return "ref*";
    }

    #endregion

    #region User/Role Cache Keys

    /// <summary>
    /// Cache key for user by ID.
    /// </summary>
    public static string UserKey(Guid userId)
    {
        return $"user{KeySeparator}{userId}";
    }

    /// <summary>
    /// Cache key for user by email.
    /// </summary>
    public static string UserByEmailKey(string email)
    {
        return $"user{KeySeparator}email{KeySeparator}{HashString(email)}";
    }

    /// <summary>
    /// Cache key for user roles.
    /// </summary>
    public static string UserRolesKey(Guid userId)
    {
        return $"user{KeySeparator}{userId}{KeySeparator}roles";
    }

    /// <summary>
    /// Cache key for user permissions.
    /// </summary>
    public static string UserPermissionsKey(Guid userId)
    {
        return $"user{KeySeparator}{userId}{KeySeparator}permissions";
    }

    /// <summary>
    /// Pattern to invalidate all user caches.
    /// </summary>
    public static string UsersPatternKey()
    {
        return "user*";
    }

    #endregion

    #region Utility Methods

    /// <summary>
    /// Generate consistent hash for long strings to keep cache keys manageable.
    /// Useful for search terms, emails, etc.
    /// </summary>
    /// <param name="input">String to hash</param>
    /// <returns>Hash string (lowercase hex)</returns>
    private static string HashString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "empty";

        using var sha = System.Security.Cryptography.SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").Substring(0, 8).ToLower();
    }

    /// <summary>
    /// Build cache key from parts with separator.
    /// Useful for dynamic key generation.
    /// </summary>
    public static string BuildKey(params object[] parts)
    {
        return string.Join(KeySeparator, parts.Select(p => p?.ToString() ?? "null"));
    }

    /// <summary>
    /// Get all cache patterns that should be invalidated for entity.
    /// Used by command handlers to know which caches to clear.
    /// </summary>
    public static IEnumerable<string> GetPatternsForEntity(string entityType)
    {
        return entityType.ToLower() switch
        {
            "patient" => new[] { PatientsPatternKey() },
            "appointment" => new[] { AppointmentsPatternKey() },
            "soapnote" => new[] { "soapnote*", "patient*soapnotes", "patient*clinical*" },
            "user" => new[] { UsersPatternKey() },
            "referencedata" => new[] { ReferenceDataPatternKey() },
            _ => new[] { entityType.ToLower() + "*" }
        };
    }

    #endregion
}

/// <summary>
/// Cache invalidation helper for command handlers.
/// Automatically clears related cache keys after data changes.
/// </summary>
public class CacheInvalidationBuilder
{
    private readonly ICacheService _cacheService;
    private readonly List<string> _keysToInvalidate = new();

    public CacheInvalidationBuilder(ICacheService cacheService)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
    }

    /// <summary>
    /// Add key to invalidation list.
    /// </summary>
    public CacheInvalidationBuilder InvalidateKey(string key)
    {
        _keysToInvalidate.Add(key);
        return this;
    }

    /// <summary>
    /// Add keys to invalidation list.
    /// </summary>
    public CacheInvalidationBuilder InvalidateKeys(params string[] keys)
    {
        _keysToInvalidate.AddRange(keys);
        return this;
    }

    /// <summary>
    /// Add pattern to invalidation list.
    /// </summary>
    public CacheInvalidationBuilder InvalidatePattern(string pattern)
    {
        _keysToInvalidate.Add(pattern);
        return this;
    }

    /// <summary>
    /// Execute all invalidations.
    /// Keys and patterns are processed separately.
    /// </summary>
    public async System.Threading.Tasks.Task ExecuteAsync(System.Threading.CancellationToken ct = default)
    {
        if (_keysToInvalidate.Count == 0)
            return;

        // Separate keys from patterns
        var keys = _keysToInvalidate.Where(k => !k.Contains("*")).ToList();
        var patterns = _keysToInvalidate.Where(k => k.Contains("*")).ToList();

        // Invalidate specific keys
        if (keys.Any())
            await _cacheService.RemoveAsync(keys, ct);

        // Invalidate patterns
        foreach (var pattern in patterns)
        {
            await _cacheService.RemoveByPatternAsync(pattern, ct);
        }
    }
}
