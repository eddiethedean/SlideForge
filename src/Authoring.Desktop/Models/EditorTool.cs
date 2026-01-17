namespace Authoring.Desktop.Models;

/// <summary>
/// Represents the currently selected editor tool.
/// </summary>
public enum EditorTool
{
    /// <summary>
    /// No tool selected - selection mode.
    /// </summary>
    None,

    /// <summary>
    /// Text object creation tool.
    /// </summary>
    Text,

    /// <summary>
    /// Image object creation tool.
    /// </summary>
    Image,

    /// <summary>
    /// Button object creation tool.
    /// </summary>
    Button
}
