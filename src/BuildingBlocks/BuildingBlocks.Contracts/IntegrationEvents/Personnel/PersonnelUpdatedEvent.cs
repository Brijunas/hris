namespace BuildingBlocks.Contracts.IntegrationEvents.Personnel;

/// <summary>
/// Integration event published when a personnel record is updated.
/// Contains identifiers and change metadata for cross-module synchronization.
/// </summary>
public sealed record PersonnelUpdatedEvent : IntegrationEvent
{
    /// <summary>
    /// The unique identifier of the updated personnel.
    /// </summary>
    public required Guid PersonnelId { get; init; }

    /// <summary>
    /// The organizational unit this personnel belongs to.
    /// </summary>
    public required Guid OrgUnitId { get; init; }

    /// <summary>
    /// Categories of fields that were updated.
    /// Allows consumers to filter relevant changes.
    /// </summary>
    public required IReadOnlyList<PersonnelChangeCategory> ChangedCategories { get; init; }

    /// <summary>
    /// Version number after the update for optimistic concurrency.
    /// </summary>
    public required int Version { get; init; }
}

/// <summary>
/// Categories of personnel data that can change.
/// Used to allow modules to react only to relevant updates.
/// </summary>
public enum PersonnelChangeCategory
{
    /// <summary>Personal information (name, contact).</summary>
    PersonalInfo,

    /// <summary>Employment details (position, department).</summary>
    Employment,

    /// <summary>Compensation and benefits.</summary>
    Compensation,

    /// <summary>Organizational assignment.</summary>
    Organization,

    /// <summary>Status changes (active, terminated, on leave).</summary>
    Status,

    /// <summary>Documents and certifications.</summary>
    Documents
}
