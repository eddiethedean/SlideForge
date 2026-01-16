using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a slide in a project.
/// </summary>
public class Slide
{
    /// <summary>
    /// Gets or sets the unique identifier of the slide.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the title of the slide.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the width of the slide in pixels.
    /// </summary>
    [JsonPropertyName("width")]
    public double Width { get; set; } = 1920;

    /// <summary>
    /// Gets or sets the height of the slide in pixels.
    /// </summary>
    [JsonPropertyName("height")]
    public double Height { get; set; } = 1080;

    /// <summary>
    /// Gets or sets the list of layers in this slide.
    /// </summary>
    [JsonPropertyName("layers")]
    public List<Layer> Layers { get; set; } = new();
}
