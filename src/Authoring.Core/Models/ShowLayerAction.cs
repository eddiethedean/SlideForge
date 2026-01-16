using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Action that shows a layer.
/// </summary>
public class ShowLayerAction : Action
{
    /// <summary>
    /// Gets the type of action.
    /// </summary>
    public override ActionType Type => ActionType.ShowLayer;

    /// <summary>
    /// Gets or sets the ID of the layer to show.
    /// </summary>
    [JsonPropertyName("layerId")]
    public string LayerId { get; set; } = string.Empty;
}
