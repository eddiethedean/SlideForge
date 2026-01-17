using Authoring.Core.Models;
using Authoring.Core.Serialization;
using Authoring.Core.Tests.Helpers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Authoring.Core.Tests.Performance;

[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[Trait("Category", "Performance")]
public class SerializationPerformanceTests
{
    private Project _smallProject = null!;
    private Project _mediumProject = null!;
    private Project _largeProject = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallProject = TestDataFactory.CreateMultiSlideProject(5);
        _mediumProject = TestDataFactory.CreateMultiSlideProject(25);
        _largeProject = TestDataFactory.CreateLargeProject(slides: 100, objectsPerSlide: 50);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetProjects))]
    public string SerializeProject(Project project)
    {
        return ProjectJsonSerializer.Serialize(project);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetProjects))]
    public Project DeserializeProject(Project project)
    {
        var json = ProjectJsonSerializer.Serialize(project);
        return ProjectJsonSerializer.Deserialize(json);
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetProjects))]
    public Project SerializeDeserializeRoundTrip(Project project)
    {
        var json = ProjectJsonSerializer.Serialize(project);
        return ProjectJsonSerializer.Deserialize(json);
    }

    public IEnumerable<object> GetProjects()
    {
        yield return new object[] { _smallProject };
        yield return new object[] { _mediumProject };
        yield return new object[] { _largeProject };
    }
}
