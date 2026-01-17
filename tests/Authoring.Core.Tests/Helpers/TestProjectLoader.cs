using System.IO;
using System.Reflection;
using Authoring.Core.Models;
using Authoring.Core.Serialization;

namespace Authoring.Core.Tests.Helpers;

/// <summary>
/// Helper for loading test project files from the TestData directory.
/// </summary>
public static class TestProjectLoader
{
    private static readonly string TestDataDirectory = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "..", "..", "..", "..", "TestData", "SampleProjects");

    /// <summary>
    /// Loads a test project from the SampleProjects directory.
    /// </summary>
    public static Project LoadProject(string fileName)
    {
        var filePath = Path.Combine(TestDataDirectory, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Test project file not found: {filePath}");
        }

        var json = File.ReadAllText(filePath);
        return ProjectJsonSerializer.Deserialize(json);
    }

    /// <summary>
    /// Loads the minimal test project.
    /// </summary>
    public static Project LoadMinimalProject()
    {
        return LoadProject("minimal-project.json");
    }

    /// <summary>
    /// Loads the complex test project.
    /// </summary>
    public static Project LoadComplexProject()
    {
        return LoadProject("complex-project.json");
    }
}
