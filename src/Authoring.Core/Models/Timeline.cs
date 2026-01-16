using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents the timeline for an object, defining when it appears and how long it is visible.
/// </summary>
public class Timeline
{
    /// <summary>
    /// Gets or sets the start time in seconds from the beginning of the slide timeline.
    /// Must be >= 0.
    /// </summary>
    [JsonPropertyName("startTime")]
    public double StartTime { get; set; }

    /// <summary>
    /// Gets or sets the duration in seconds that the object is visible.
    /// Must be > 0.
    /// </summary>
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
}
