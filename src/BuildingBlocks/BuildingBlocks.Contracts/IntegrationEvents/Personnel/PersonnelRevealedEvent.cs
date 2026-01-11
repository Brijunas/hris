namespace BuildingBlocks.Contracts.IntegrationEvents.Personnel;

/// <summary>
/// Integration event published when personnel data is accessed or revealed.
/// Contains only metadata for audit purposes - no sensitive data is included.
/// This supports data access logging for GDPR and compliance requirements.
/// </summary>
public sealed record PersonnelRevealedEvent : IntegrationEvent
{
    /// <summary>
    /// The unique identifier of the personnel whose data was accessed.
    /// </summary>
    public required Guid PersonnelId { get; init; }

    /// <summary>
    /// The type of data that was revealed.
    /// </summary>
    public required RevealedDataType DataType { get; init; }

    /// <summary>
    /// The reason or purpose for accessing the data.
    /// </summary>
    public required string AccessReason { get; init; }

    /// <summary>
    /// The context in which the data was revealed (e.g., API endpoint, report).
    /// </summary>
    public required string AccessContext { get; init; }

    /// <summary>
    /// Whether the access was for bulk/export operations.
    /// </summary>
    public bool IsBulkAccess { get; init; }
}

/// <summary>
/// Types of sensitive personnel data that can be revealed.
/// </summary>
public enum RevealedDataType
{
    /// <summary>Basic profile information.</summary>
    BasicProfile,

    /// <summary>Personal identification data (SSN, ID numbers).</summary>
    PersonalIdentification,

    /// <summary>Contact information (address, phone, email).</summary>
    ContactInformation,

    /// <summary>Financial data (salary, bank details).</summary>
    FinancialData,

    /// <summary>Medical or health information.</summary>
    HealthInformation,

    /// <summary>Performance evaluations.</summary>
    PerformanceData,

    /// <summary>Full personnel record.</summary>
    FullRecord
}
