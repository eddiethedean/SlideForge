using System;
using System.Linq;
using Authoring.Core.Models;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service implementation for managing slides within a project.
/// </summary>
public class SlideManagementService : ISlideManagementService
{
    /// <summary>
    /// Creates a new slide and adds it to the project.
    /// </summary>
    public Slide CreateSlide(Project project)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        var newSlide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"Slide {project.Slides.Count + 1}",
            Width = 1920,
            Height = 1080
        };

        var baseLayer = new Layer
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Base Layer",
            Visible = true
        };

        newSlide.Layers.Add(baseLayer);
        project.AddSlide(newSlide);
        return newSlide;
    }

    /// <summary>
    /// Deletes a slide from the project.
    /// </summary>
    public Slide? DeleteSlide(Project project, Slide slideToDelete)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));
        if (slideToDelete == null)
            throw new ArgumentNullException(nameof(slideToDelete));

        project.Slides.Remove(slideToDelete);
        
        // Return the next slide to display, or the first remaining slide
        return project.Slides.FirstOrDefault(s => s != slideToDelete) 
            ?? project.Slides.FirstOrDefault();
    }

    /// <summary>
    /// Duplicates a slide, including all layers and objects.
    /// </summary>
    public Slide DuplicateSlide(Project project, Slide sourceSlide)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));
        if (sourceSlide == null)
            throw new ArgumentNullException(nameof(sourceSlide));

        var newSlide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = $"{sourceSlide.Title} (Copy)",
            Width = sourceSlide.Width,
            Height = sourceSlide.Height
        };

        foreach (var sourceLayer in sourceSlide.Layers)
        {
            var newLayer = new Layer
            {
                Id = Guid.NewGuid().ToString(),
                Name = sourceLayer.Name,
                Visible = sourceLayer.Visible
            };

            foreach (var sourceObject in sourceLayer.Objects)
            {
                var clonedObject = sourceObject.Clone() as SlideObject;
                if (clonedObject != null)
                {
                    clonedObject.Id = Guid.NewGuid().ToString();
                    newLayer.Objects.Add(clonedObject);
                }
            }

            newSlide.Layers.Add(newLayer);
        }

        project.AddSlide(newSlide);
        return newSlide;
    }
}
