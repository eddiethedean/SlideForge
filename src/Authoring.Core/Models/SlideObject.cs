using System.Text.Json.Serialization;
using System.Text.Json;

namespace Authoring.Core.Models;

/// <summary>
/// Base class for all objects that can be placed on a slide.
/// </summary>
[JsonDerivedType(typeof(TextObject), "text")]
[JsonDerivedType(typeof(ImageObject), "image")]
[JsonDerivedType(typeof(ButtonObject), "button")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "objectType")]
public abstract class SlideObject
{
    /// <summary>
    /// Gets or sets the unique identifier of the object.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the object.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the X coordinate of the object's position.
    /// </summary>
    [JsonPropertyName("x")]
    public double X { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the object's position.
    /// </summary>
    [JsonPropertyName("y")]
    public double Y { get; set; }

    /// <summary>
    /// Gets or sets the width of the object.
    /// </summary>
    [JsonPropertyName("width")]
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the height of the object.
    /// </summary>
    [JsonPropertyName("height")]
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets whether the object is visible.
    /// </summary>
    [JsonPropertyName("visible")]
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Gets or sets the timeline for this object. Null if the object is always visible.
    /// </summary>
    [JsonPropertyName("timeline")]
    public Timeline? Timeline { get; set; }

    /// <summary>
    /// Gets or sets the list of triggers attached to this object.
    /// </summary>
    [JsonPropertyName("triggers")]
    public List<Trigger> Triggers { get; set; } = new();
}
