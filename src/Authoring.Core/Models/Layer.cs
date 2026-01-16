using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a layer that contains objects on a slide.
/// </summary>
public class Layer
{
    /// <summary>
    /// Gets or sets the unique identifier of the layer.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the layer.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the layer is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the list of objects contained in this layer.
    /// </summary>
    [JsonPropertyName("objects")]
    public List<SlideObject> Objects { get; set; } = new();
}
