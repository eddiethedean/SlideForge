using System;
using System.Linq;
using Authoring.Core.Models;
using Authoring.Desktop.Models;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service implementation for managing objects within slides.
/// </summary>
public class ObjectManagementService : IObjectManagementService
{
    /// <summary>
    /// Creates a new object at the specified position on a slide.
    /// </summary>
    public SlideObject? CreateObject(Slide slide, EditorTool tool, double x, double y)
    {
        if (slide == null)
            throw new ArgumentNullException(nameof(slide));
        if (tool == EditorTool.None)
            return null;

        var baseLayer = slide.Layers.FirstOrDefault();
        if (baseLayer == null)
            return null;

        SlideObject? newObject = tool switch
        {
            EditorTool.Text => new TextObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Text Object",
                X = x,
                Y = y,
                Width = 200,
                Height = 50,
                Text = "Text",
                FontSize = 16,
                Visible = true
            },
            EditorTool.Image => new ImageObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Image Object",
                X = x,
                Y = y,
                Width = 100,
                Height = 100,
                SourcePath = "",
                Visible = true
            },
            EditorTool.Button => new ButtonObject
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Button Object",
                X = x,
                Y = y,
                Width = 150,
                Height = 40,
                Label = "Button",
                Enabled = true,
                Visible = true
            },
            _ => null
        };

        if (newObject != null)
        {
            baseLayer.Objects.Add(newObject);
        }

        return newObject;
    }

    /// <summary>
    /// Deletes an object from a slide.
    /// </summary>
    public void DeleteObject(Slide slide, SlideObject objectToDelete)
    {
        if (slide == null)
            throw new ArgumentNullException(nameof(slide));
        if (objectToDelete == null)
            throw new ArgumentNullException(nameof(objectToDelete));

        foreach (var layer in slide.Layers)
        {
            if (layer.Objects.Remove(objectToDelete))
            {
                break; // Object found and removed
            }
        }
    }
}
