using System.Text.Json.Serialization;

namespace Authoring.Core.Models;

/// <summary>
/// Represents a complete SlideForge e-learning project.
/// </summary>
public class Project
{
    /// <summary>
    /// Initializes a new instance of the Project class.
    /// </summary>
    public Project()
    {
        Id = Guid.NewGuid().ToString();
        CreatedAt = DateTime.UtcNow;
        ModifiedAt = DateTime.UtcNow;
        Variables = new List<Variable>();
        Slides = new List<Slide>();
    }

    /// <summary>
    /// Gets or sets the unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the project.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version of the project.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    /// <summary>
    /// Gets or sets the author of the project.
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the project was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the project was last modified.
    /// </summary>
    [JsonPropertyName("modifiedAt")]
    public DateTime ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of variables defined in the project.
    /// </summary>
    [JsonPropertyName("variables")]
    public List<Variable> Variables { get; set; }

    /// <summary>
    /// Gets or sets the list of slides in the project.
    /// </summary>
    [JsonPropertyName("slides")]
    public List<Slide> Slides { get; set; }

    /// <summary>
    /// Adds a slide to the project.
    /// </summary>
    /// <param name="slide">The slide to add.</param>
    public void AddSlide(Slide slide)
    {
        if (slide == null)
            throw new ArgumentNullException(nameof(slide));

        Slides.Add(slide);
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a variable to the project.
    /// </summary>
    /// <param name="variable">The variable to add.</param>
    public void AddVariable(Variable variable)
    {
        if (variable == null)
            throw new ArgumentNullException(nameof(variable));

        Variables.Add(variable);
        ModifiedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets a slide by its ID.
    /// </summary>
    /// <param name="id">The ID of the slide to find.</param>
    /// <returns>The slide with the specified ID, or null if not found.</returns>
    public Slide? GetSlideById(string id)
    {
        return Slides.FirstOrDefault(s => s.Id == id);
    }

    /// <summary>
    /// Gets a variable by its ID.
    /// </summary>
    /// <param name="id">The ID of the variable to find.</param>
    /// <returns>The variable with the specified ID, or null if not found.</returns>
    public Variable? GetVariableById(string id)
    {
        return Variables.FirstOrDefault(v => v.Id == id);
    }
}
