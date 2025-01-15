namespace RefApi.Data;

/// <summary>
/// Serves as the base class for all domain entities, including common auditing information
/// like creation and update metadata.
/// </summary>
public class AuditableEntity
{
    /// <summary>Gets or sets the entity's unique identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Gets or sets the date and time the entity was created.</summary>
    /// <remarks>This property is set automatically upon the creation of the entity.</remarks>
    public DateTime CreatedAt { get; init; }

    /// <summary>Gets or sets the identifier of the user who created the entity.</summary>
    /// <remarks>Should correlate to the authenticated user's identifier in the system.</remarks>
    public string CreatedBy { get; init; } = null!;

    /// <summary>Gets or sets the date and time the entity was last updated.</summary>
    /// <remarks>This property should be set each time the entity undergoes a significant update.</remarks>
    public DateTime? UpdatedAt { get; init; }

    /// <summary>Gets or sets the identifier of the user who last updated the entity.</summary>
    /// <remarks>Should correlate to the authenticated user's identifier in the system at the time of update.</remarks>
    public string? UpdatedBy { get; init; }
}