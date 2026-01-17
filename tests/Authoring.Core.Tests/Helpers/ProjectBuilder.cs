using Authoring.Core.Models;

namespace Authoring.Core.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test projects.
/// </summary>
public class ProjectBuilder
{
    private string _name = "Test Project";
    private string? _author;
    private string _version = "1.0.0";
    private readonly List<Slide> _slides = new();
    private readonly List<Variable> _variables = new();

    public ProjectBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProjectBuilder WithAuthor(string? author)
    {
        _author = author;
        return this;
    }

    public ProjectBuilder WithVersion(string version)
    {
        _version = version;
        return this;
    }

    public ProjectBuilder WithSlide(Slide slide)
    {
        _slides.Add(slide);
        return this;
    }

    public ProjectBuilder WithSlide(Action<SlideBuilder> configure)
    {
        var builder = new SlideBuilder();
        configure(builder);
        _slides.Add(builder.Build());
        return this;
    }

    public ProjectBuilder WithVariable(Variable variable)
    {
        _variables.Add(variable);
        return this;
    }

    public ProjectBuilder WithVariable(string id, string name, VariableType type, object? defaultValue = null)
    {
        _variables.Add(new Variable
        {
            Id = id,
            Name = name,
            Type = type,
            DefaultValue = defaultValue
        });
        return this;
    }

    public Project Build()
    {
        var project = new Project
        {
            Name = _name,
            Author = _author,
            Version = _version
        };

        foreach (var variable in _variables)
        {
            project.AddVariable(variable);
        }

        foreach (var slide in _slides)
        {
            project.AddSlide(slide);
        }

        return project;
    }

    public static ProjectBuilder Create() => new();
}
