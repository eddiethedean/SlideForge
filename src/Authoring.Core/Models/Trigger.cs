using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a trigger that fires actions when a specific event occurs.
/// </summary>
public class Trigger
{
    /// <summary>
    /// Gets or sets the unique identifier of the trigger.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of trigger event.
    /// </summary>
    [JsonPropertyName("type")]
    public TriggerType Type { get; set; }

    /// <summary>
    /// Gets or sets the ID of the object this trigger is attached to. Null for slide-level triggers.
    /// </summary>
    [JsonPropertyName("objectId")]
    public string? ObjectId { get; set; }

    /// <summary>
    /// Gets or sets the list of actions to execute when this trigger fires.
    /// </summary>
    [JsonPropertyName("actions")]
    public List<Action> Actions { get; set; } = new();
}
