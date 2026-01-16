using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Action that navigates to a specific slide.
/// </summary>
public class NavigateToSlideAction : Action
{
    /// <summary>
    /// Gets the type of action.
    /// </summary>
    public override ActionType Type => ActionType.NavigateToSlide;

    /// <summary>
    /// Gets or sets the ID of the target slide to navigate to.
    /// </summary>
    [JsonPropertyName("targetSlideId")]
    public string TargetSlideId { get; set; } = string.Empty;
}
