using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a button object that can be placed on a slide.
/// </summary>
public class ButtonObject : SlideObject
{
    /// <summary>
    /// Gets or sets the label text displayed on the button.
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the button is enabled and clickable.
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Creates a shallow copy of this ButtonObject.
    /// </summary>
    public override object Clone()
    {
        var clone = new ButtonObject
        {
            Label = Label,
            Enabled = Enabled
        };
        CopyBasePropertiesTo(clone);
        return clone;
    }
}
