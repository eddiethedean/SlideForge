using Authoring.Core.Models;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service for managing slides within a project.
/// </summary>
public interface ISlideManagementService
{
    /// <summary>
    /// Creates a new slide and adds it to the project.
    /// </summary>
    /// <param name="project">The project to add the slide to.</param>
    /// <returns>The newly created slide.</returns>
    Slide CreateSlide(Project project);

    /// <summary>
    /// Deletes a slide from the project.
    /// </summary>
    /// <param name="project">The project containing the slide.</param>
    /// <param name="slideToDelete">The slide to delete.</param>
    /// <returns>The slide to display after deletion (next slide or first remaining slide).</returns>
    Slide? DeleteSlide(Project project, Slide slideToDelete);

    /// <summary>
    /// Duplicates a slide, including all layers and objects.
    /// </summary>
    /// <param name="project">The project containing the source slide.</param>
    /// <param name="sourceSlide">The slide to duplicate.</param>
    /// <returns>The newly created duplicate slide.</returns>
    Slide DuplicateSlide(Project project, Slide sourceSlide);
}
