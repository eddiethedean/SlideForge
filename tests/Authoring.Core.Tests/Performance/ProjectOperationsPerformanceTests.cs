using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Authoring.Core.Tests.Performance;

[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[Trait("Category", "Performance")]
public class ProjectOperationsPerformanceTests
{
    private Project _project = null!;
    private List<Slide> _slides = null!;

    [GlobalSetup]
    public void Setup()
    {
        _project = TestDataFactory.CreateMultiSlideProject(100);
        _slides = new List<Slide>();
        for (int i = 0; i < 50; i++)
        {
            _slides.Add(SlideBuilder.Create()
                .WithTitle($"Slide {i}")
                .Build());
        }
    }

    [Benchmark]
    public void AddMultipleSlides()
    {
        var project = new Project { Name = "Test" };
        foreach (var slide in _slides)
        {
            project.AddSlide(slide);
        }
    }

    [Benchmark]
    public Slide? GetSlideById()
    {
        var targetId = _project.Slides[_project.Slides.Count / 2].Id;
        return _project.GetSlideById(targetId);
    }

    [Benchmark]
    public Variable? GetVariableById()
    {
        if (_project.Variables.Count == 0)
        {
            _project.AddVariable(new Variable
            {
                Id = "test",
                Name = "Test",
                Type = VariableType.Number,
                DefaultValue = 0
            });
        }
        return _project.GetVariableById("test");
    }

    [Benchmark]
    public void SearchObjectsInProject()
    {
        // Simulate searching for objects by name
        var results = new List<SlideObject>();
        foreach (var slide in _project.Slides)
        {
            foreach (var layer in slide.Layers)
            {
                results.AddRange(layer.Objects.Where(o => o.Name.Contains("Text")));
            }
        }
    }
}
