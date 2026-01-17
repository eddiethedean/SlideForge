using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a text object that can be placed on a slide.
/// </summary>
public class TextObject : SlideObject
{
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the font family name.
    /// </summary>
    [JsonPropertyName("fontFamily")]
    public string FontFamily { get; set; } = "Arial";

    /// <summary>
    /// Gets or sets the font size in points.
    /// </summary>
    [JsonPropertyName("fontSize")]
    public double FontSize { get; set; } = 12.0;

    /// <summary>
    /// Gets or sets the text color as a hex string (e.g., "#FF0000" for red).
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = "#000000";

    /// <summary>
    /// Creates a shallow copy of this TextObject.
    /// </summary>
    public override object Clone()
    {
        var clone = new TextObject
        {
            Text = Text,
            FontFamily = FontFamily,
            FontSize = FontSize,
            Color = Color
        };
        CopyBasePropertiesTo(clone);
        return clone;
    }
}
