using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Action that sets the value of a variable.
/// </summary>
public class SetVariableAction : Action
{
    /// <summary>
    /// Gets the type of action.
    /// </summary>
    public override ActionType Type => ActionType.SetVariable;

    /// <summary>
    /// Gets or sets the ID of the variable to set.
    /// </summary>
    [JsonPropertyName("variableId")]
    public string VariableId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value to assign to the variable. The type should match the variable's type.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonConverter(typeof(Serialization.ObjectJsonConverter))]
    public object? Value { get; set; }
}
