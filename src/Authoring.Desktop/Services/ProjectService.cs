using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Authoring.Core.Models;
using Authoring.Core.Serialization;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service implementation for project file operations.
/// </summary>
public class ProjectService : IProjectService
{
    public async Task<Project?> OpenProjectAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var project = ProjectJsonSerializer.Deserialize(json);
            return project;
        }
        catch
        {
            return null;
        }
    }

    public async Task SaveProjectAsync(Project project, string filePath)
    {
        project.ModifiedAt = DateTime.UtcNow;
        var json = ProjectJsonSerializer.Serialize(project);
        await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
    }

    public Project CreateNewProject(string name, string? author = null)
    {
        var project = new Project
        {
            Name = name,
            Author = author
        };

        // Create a default slide
        var defaultSlide = new Slide
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Slide 1",
            Width = 1920,
            Height = 1080
        };

        // Create a default base layer
        var baseLayer = new Layer
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Base Layer",
            Visible = true
        };

        defaultSlide.Layers.Add(baseLayer);
        project.AddSlide(defaultSlide);

        return project;
    }
}
