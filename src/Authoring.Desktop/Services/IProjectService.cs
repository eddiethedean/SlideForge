using System.Threading.Tasks;
using Authoring.Core.Models;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service interface for project file operations.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Opens a project from a file path.
    /// </summary>
    /// <param name="filePath">The path to the project file.</param>
    /// <returns>The loaded project, or null if loading failed.</returns>
    Task<Project?> OpenProjectAsync(string filePath);

    /// <summary>
    /// Saves a project to a file path.
    /// </summary>
    /// <param name="project">The project to save.</param>
    /// <param name="filePath">The path where the project should be saved.</param>
    Task SaveProjectAsync(Project project, string filePath);

    /// <summary>
    /// Creates a new project with the specified name and author.
    /// </summary>
    /// <param name="name">The name of the project.</param>
    /// <param name="author">The author of the project (optional).</param>
    /// <returns>A new project instance with a default slide.</returns>
    Project CreateNewProject(string name, string? author = null);
}
