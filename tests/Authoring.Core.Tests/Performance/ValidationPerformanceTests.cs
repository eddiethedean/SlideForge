using Authoring.Core.Models;
using Authoring.Core.Validation;
using Authoring.Core.Tests.Helpers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Authoring.Core.Tests.Performance;

[SimpleJob(RuntimeMoniker.Net90)]
[MemoryDiagnoser]
[Trait("Category", "Performance")]
public class ValidationPerformanceTests
{
    private Project _smallProject = null!;
    private Project _mediumProject = null!;
    private Project _largeProject = null!;
    private Project _invalidProject = null!;

    [GlobalSetup]
    public void Setup()
    {
        _smallProject = TestDataFactory.CreateMultiSlideProject(10);
        _mediumProject = TestDataFactory.CreateMultiSlideProject(50);
        _largeProject = TestDataFactory.CreateLargeProject(slides: 200, objectsPerSlide: 100);
        _invalidProject = TestDataFactory.CreateInvalidProject();
    }

    [Benchmark]
    [ArgumentsSource(nameof(GetProjects))]
    public List<string> ValidateProject(Project project)
    {
        return ProjectValidator.ValidateProject(project);
    }

    public IEnumerable<object> GetProjects()
    {
        yield return new object[] { _smallProject };
        yield return new object[] { _mediumProject };
        yield return new object[] { _largeProject };
        yield return new object[] { _invalidProject };
    }
}
