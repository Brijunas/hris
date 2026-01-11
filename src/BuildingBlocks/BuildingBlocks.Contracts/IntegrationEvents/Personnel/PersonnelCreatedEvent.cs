namespace BuildingBlocks.Contracts.IntegrationEvents.Personnel;

/// <summary>
/// Integration event published when a new personnel record is created.
/// Contains essential identifiers for cross-module synchronization.
/// </summary>
public sealed record PersonnelCreatedEvent : IntegrationEvent
{
    /// <summary>
    /// The unique identifier of the newly created personnel.
    /// </summary>
    public required Guid PersonnelId { get; init; }

    /// <summary>
    /// The organizational unit this personnel belongs to.
    /// </summary>
    public required Guid OrgUnitId { get; init; }

    /// <summary>
    /// The employee number or personnel code.
    /// </summary>
    public required string PersonnelNumber { get; init; }

    /// <summary>
    /// The employment start date.
    /// </summary>
    public required DateOnly StartDate { get; init; }

    /// <summary>
    /// The position or job title ID.
    /// </summary>
    public Guid? PositionId { get; init; }
}
