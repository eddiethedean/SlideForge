using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents an image object that can be placed on a slide.
/// </summary>
public class ImageObject : SlideObject
{
    /// <summary>
    /// Gets or sets the source path or URL of the image.
    /// </summary>
    [JsonPropertyName("sourcePath")]
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to maintain the aspect ratio when resizing.
    /// </summary>
    [JsonPropertyName("maintainAspectRatio")]
    public bool MaintainAspectRatio { get; set; } = true;
}
