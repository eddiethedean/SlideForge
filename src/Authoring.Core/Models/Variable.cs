using System.Text.Json.Serialization;
using Authoring.Core.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a variable that can store state during project execution.
/// </summary>
public class Variable
{
    /// <summary>
    /// Gets or sets the unique identifier of the variable.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the variable.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type of the variable.
    /// </summary>
    [JsonPropertyName("type")]
    public VariableType Type { get; set; }

    /// <summary>
    /// Gets or sets the default value of the variable. The type should match the VariableType.
    /// </summary>
    [JsonPropertyName("defaultValue")]
    [JsonConverter(typeof(ObjectJsonConverter))]
    public object? DefaultValue { get; set; }
}
