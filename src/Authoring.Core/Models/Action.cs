using System.Text.Json.Serialization;
using System.Text.Json;

namespace Authoring.Core.Models;

/// <summary>
/// Base class for all actions that can be executed by triggers.
/// </summary>
[JsonDerivedType(typeof(NavigateToSlideAction), "navigateToSlide")]
[JsonDerivedType(typeof(SetVariableAction), "setVariable")]
[JsonDerivedType(typeof(ShowLayerAction), "showLayer")]
[JsonDerivedType(typeof(HideLayerAction), "hideLayer")]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "actionType")]
public abstract class Action
{
    /// <summary>
    /// Gets the type of action.
    /// </summary>
    [JsonIgnore]
    public abstract ActionType Type { get; }
}
