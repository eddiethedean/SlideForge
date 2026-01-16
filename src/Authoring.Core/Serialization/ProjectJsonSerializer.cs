using System.Text.Json;
using System.Text.Json.Serialization;
using Authoring.Core.Models;

namespace Authoring.Core.Serialization;

/// <summary>
/// Provides JSON serialization and deserialization for Project objects.
/// </summary>
public static class ProjectJsonSerializer
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    /// <summary>
    /// Serializes a Project to a JSON string.
    /// </summary>
    /// <param name="project">The project to serialize.</param>
    /// <returns>A JSON string representation of the project.</returns>
    /// <exception cref="ArgumentNullException">Thrown when project is null.</exception>
    public static string Serialize(Project project)
    {
        if (project == null)
            throw new ArgumentNullException(nameof(project));

        return JsonSerializer.Serialize(project, Options);
    }

    /// <summary>
    /// Deserializes a Project from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>A Project instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when json is null.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid.</exception>
    public static Project Deserialize(string json)
    {
        if (json == null)
            throw new ArgumentNullException(nameof(json));

        return JsonSerializer.Deserialize<Project>(json, Options)
            ?? throw new JsonException("Deserialization resulted in null.");
    }
}
