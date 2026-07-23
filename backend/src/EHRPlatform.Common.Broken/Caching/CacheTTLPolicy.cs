using System;

namespace EHRPlatform.Common.Caching;

/// <summary>
/// Cache TTL (Time-To-Live) policies for different data types.
/// Defines how long different cached data should persist in Redis.
/// 
/// Strategy:
/// - **Short-lived** (1-5 min): Frequently updated, session data, temporary results
/// - **Medium-lived** (5-30 min): Reference data, search results, patient lists
/// - **Long-lived** (1+ hours): Static data, configurations, rare changes
/// - **Permanent**: Never expires naturally (e.g., audit logs reference data)
/// 
/// HIPAA Note:
/// Clinical data caches should be relatively short-lived to ensure
/// reasonably current information for patient care.
/// </summary>
public static class CacheTTLPolicy
{
    /// <summary>
    /// Session and temporary data (1 minute).
    /// For: Active user sessions, temporary computation results, OTP cache.
    /// </summary>
    public static TimeSpan ShortLived = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Frequently used data (5 minutes).
    /// For: Patient searches, appointment lists, current vital signs.
    /// Short enough for clinical freshness, long enough for performance.
    /// </summary>
    public static TimeSpan Standard = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Medium-retention data (15 minutes).
    /// For: Patient demographics, appointment schedules, medication lists.
    /// </summary>
    public static TimeSpan MediumLived = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Long-retention data (1 hour).
    /// For: Reference data (ICD-10 codes, CPT codes), configuration, static lookups.
    /// </summary>
    public static TimeSpan LongLived = TimeSpan.FromHours(1);

    /// <summary>
    /// Very long retention (6 hours).
    /// For: Rarely changing master data, provider schedules, facility information.
    /// </summary>
    public static TimeSpan VeryLongLived = TimeSpan.FromHours(6);

    /// <summary>
    /// Extended retention (24 hours).
    /// For: Daily statistics, accumulated data, report cache.
    /// </summary>
    public static TimeSpan Daily = TimeSpan.FromHours(24);

    /// <summary>
    /// No expiration (effectively permanent).
    /// For: Critical reference data, configuration that rarely changes.
    /// WARNING: Only use for truly immutable data.
    /// </summary>
    public static TimeSpan? NoExpiration = null;

    /// <summary>
    /// Get recommended TTL for specific data type.
    /// </summary>
    public static TimeSpan GetPolicyForDataType(CacheDataType dataType) => dataType switch
    {
        CacheDataType.Session => ShortLived,
        CacheDataType.UserData => Standard,
        CacheDataType.PatientData => MediumLived,
        CacheDataType.ClinicalData => ShortLived, // Fresh clinical data critical
        CacheDataType.AppointmentData => MediumLived,
        CacheDataType.MedicationData => MediumLived,
        CacheDataType.ReferenceData => LongLived,
        CacheDataType.Configuration => VeryLongLived,
        CacheDataType.StaticData => Daily,
        CacheDataType.Temporary => ShortLived,
        CacheDataType.SearchResults => Standard,
        _ => Standard
    };

    /// <summary>
    /// Get TTL based on query result count (adaptive caching).
    /// Smaller result sets (more specific queries) cache longer.
    /// Larger result sets (searches) cache shorter.
    /// </summary>
    public static TimeSpan GetAdaptiveTTL(int resultCount) => resultCount switch
    {
        1 => LongLived,           // Single entity - cache longer
        <= 10 => MediumLived,    // Small result set
        <= 50 => Standard,       // Medium result set
        _ => ShortLived          // Large result set - shorter cache
    };
}

/// <summary>
/// Data type enumeration for cache policy selection.
/// </summary>
public enum CacheDataType
{
    /// <summary>User session data</summary>
    Session,

    /// <summary>User profile and permissions</summary>
    UserData,

    /// <summary>Patient demographics and MRN</summary>
    PatientData,

    /// <summary>Clinical data: vital signs, SOAP notes, diagnoses</summary>
    ClinicalData,

    /// <summary>Appointment schedules</summary>
    AppointmentData,

    /// <summary>Medications and prescriptions</summary>
    MedicationData,

    /// <summary>Reference data: ICD-10, CPT, drug codes</summary>
    ReferenceData,

    /// <summary>Application configuration</summary>
    Configuration,

    /// <summary>Static data: facilities, providers</summary>
    StaticData,

    /// <summary>Temporary/transient data</summary>
    Temporary,

    /// <summary>Search query results</summary>
    SearchResults
}

/// <summary>
/// Cache invalidation strategies.
/// Defines when cache entries should be cleared.
/// </summary>
public static class CacheInvalidationStrategy
{
    /// <summary>
    /// Invalidate on any update to the entity.
    /// Conservative strategy for critical data.
    /// </summary>
    public static readonly string OnUpdate = "OnUpdate";

    /// <summary>
    /// Invalidate on create, update, or delete.
    /// Standard strategy for most entities.
    /// </summary>
    public static readonly string OnMutation = "OnMutation";

    /// <summary>
    /// Invalidate all related entities.
    /// For cascade operations (e.g., updating patient invalidates all patient data).
    /// </summary>
    public static readonly string Cascade = "Cascade";

    /// <summary>
    /// Invalidate on schedule only.
    /// For data that updates independently (e.g., daily reports at midnight).
    /// </summary>
    public static readonly string Scheduled = "Scheduled";

    /// <summary>
    /// Never invalidate - let TTL expire naturally.
    /// For immutable or very stable data.
    /// </summary>
    public static readonly string Never = "Never";

    /// <summary>
    /// Invalidate when message received from event bus.
    /// For distributed invalidation across services.
    /// </summary>
    public static readonly string EventDriven = "EventDriven";
}
