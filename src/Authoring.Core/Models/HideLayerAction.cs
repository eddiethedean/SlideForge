using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Action that hides a layer.
/// </summary>
public class HideLayerAction : Action
{
    /// <summary>
    /// Gets the type of action.
    /// </summary>
    public override ActionType Type => ActionType.HideLayer;

    /// <summary>
    /// Gets or sets the ID of the layer to hide.
    /// </summary>
    [JsonPropertyName("layerId")]
    public string LayerId { get; set; } = string.Empty;
}
