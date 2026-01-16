namespace Authoring.Core.Models;

/// <summary>
/// Represents the type of action that can be executed by a trigger.
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Navigate to a different slide.
    /// </summary>
    NavigateToSlide,

    /// <summary>
    /// Set the value of a variable.
    /// </summary>
    SetVariable,

    /// <summary>
    /// Show a layer.
    /// </summary>
    ShowLayer,

    /// <summary>
    /// Hide a layer.
    /// </summary>
    HideLayer
}
