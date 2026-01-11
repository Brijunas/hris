namespace BuildingBlocks.Contracts.IntegrationEvents.Audit;

/// <summary>
/// Integration event published when an audit checkpoint is created.
/// Represents a significant auditable action in the system.
/// </summary>
public sealed record AuditCheckpointCreatedEvent : IntegrationEvent
{
    /// <summary>
    /// The unique identifier of the audit checkpoint.
    /// </summary>
    public required Guid CheckpointId { get; init; }

    /// <summary>
    /// The type of action that was audited.
    /// </summary>
    public required AuditActionType ActionType { get; init; }

    /// <summary>
    /// The type of entity that was affected.
    /// </summary>
    public required string EntityType { get; init; }

    /// <summary>
    /// The identifier of the affected entity.
    /// </summary>
    public required string EntityId { get; init; }

    /// <summary>
    /// The module where the action occurred.
    /// </summary>
    public required string ModuleName { get; init; }

    /// <summary>
    /// A human-readable description of the action.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// The severity level of this audit event.
    /// </summary>
    public required AuditSeverity Severity { get; init; }

    /// <summary>
    /// Additional metadata as key-value pairs.
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Types of auditable actions.
/// </summary>
public enum AuditActionType
{
    /// <summary>Entity was created.</summary>
    Create,

    /// <summary>Entity was read/accessed.</summary>
    Read,

    /// <summary>Entity was updated.</summary>
    Update,

    /// <summary>Entity was deleted.</summary>
    Delete,

    /// <summary>Data was exported.</summary>
    Export,

    /// <summary>Data was imported.</summary>
    Import,

    /// <summary>User authentication event.</summary>
    Authentication,

    /// <summary>Authorization check performed.</summary>
    Authorization,

    /// <summary>Configuration change.</summary>
    Configuration,

    /// <summary>System operation.</summary>
    System
}

/// <summary>
/// Severity levels for audit events.
/// </summary>
public enum AuditSeverity
{
    /// <summary>Informational, routine operation.</summary>
    Info,

    /// <summary>Notable action requiring attention.</summary>
    Notice,

    /// <summary>Potentially concerning action.</summary>
    Warning,

    /// <summary>Critical action requiring immediate review.</summary>
    Critical
}
