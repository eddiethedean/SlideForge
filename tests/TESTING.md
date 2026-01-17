# Testing Guidelines and Best Practices

This document outlines testing standards, patterns, and best practices for the SlideForge project.

## Test Structure

Tests are organized in two main projects:
- `Authoring.Core.Tests` - Tests for core domain models, serialization, and validation
- `Authoring.Desktop.Tests` - Tests for desktop UI, ViewModels, services, and controls

## Test Categories

Tests are categorized using xUnit traits:

- **Unit Tests** - Test individual components in isolation
- **Integration Tests** - Test component interactions and workflows
- **UI Tests** - Test UI components using Avalonia.Headless
- **Property-Based Tests** - Test invariants and properties using FsCheck
- **Performance Tests** - Benchmark performance using BenchmarkDotNet

## Test Naming Conventions

Use descriptive test names following the pattern: `MethodName_Scenario_ExpectedBehavior`

Example:
```csharp
[Fact]
public void CreateObjectAtPosition_WithTextTool_CreatesTextObject()
```

## Test Organization Patterns

### Arrange-Act-Assert (AAA)

Always use the AAA pattern:
```csharp
[Fact]
public void ExampleTest()
{
    // Arrange
    var project = ProjectBuilder.Create().WithName("Test").Build();

    // Act
    var result = SomeOperation(project);

    // Assert
    Assert.Equal(expected, result);
}
```

### Using Test Builders

Use builders instead of manual object creation:

**Good:**
```csharp
var project = ProjectBuilder.Create()
    .WithName("Test")
    .WithSlide(s => s.WithTitle("Slide 1"))
    .Build();
```

**Avoid:**
```csharp
var project = new Project { Name = "Test" };
var slide = new Slide { Id = Guid.NewGuid().ToString(), Title = "Slide 1", ... };
project.AddSlide(slide);
```

### Using Test Factories

For common scenarios, use `TestDataFactory`:

```csharp
var project = TestDataFactory.CreateComplexProject();
var largeProject = TestDataFactory.CreateLargeProject(slides: 100);
```

### Using Assertion Extensions

Use custom assertion helpers for clearer tests:

```csharp
AssertExtensions.AssertProjectsEqual(expected, actual);
ProjectAssertions.AssertProjectIsValid(project);
ViewModelAssertions.AssertProjectLoaded(viewModel, project);
```

## Test Data Management

Test data files are located in `tests/TestData/SampleProjects/`. Use `TestProjectLoader` to load them:

```csharp
var project = TestProjectLoader.LoadMinimalProject();
```

## Writing New Tests

1. Identify the test category (Unit/Integration/UI/Property-Based)
2. Use appropriate builders or factories
3. Follow AAA pattern
4. Use descriptive test names
5. Add traits for categorization if needed
6. Use assertion extensions when available

## Performance Testing

Performance tests should:
- Be in the `Performance` folder
- Use BenchmarkDotNet attributes
- Test realistic scenarios with various data sizes
- Not run as part of regular test suite (use separate command)

Run performance tests with:
```bash
dotnet test --filter "Category=Performance"
```

## Property-Based Testing

Property-based tests use FsCheck to test invariants:
- Should supplement, not replace, unit tests
- Focus on properties that should hold for all valid inputs
- Use custom generators for domain-specific types

## UI Testing

UI tests use Avalonia.Headless:
- Test view instantiation and basic rendering
- Verify property bindings
- Test command execution
- Should not rely on full UI rendering (use headless mode)

## Best Practices

1. **Keep tests fast** - Unit tests should complete in < 1s
2. **Test one thing** - Each test should verify one behavior
3. **Use meaningful names** - Test names should clearly describe what is being tested
4. **Avoid test interdependencies** - Tests should be independent and runnable in any order
5. **Clean up resources** - Use `IDisposable` for test cleanup
6. **Use mocks sparingly** - Prefer real objects when possible
7. **Test edge cases** - Include boundary conditions and error scenarios

## Running Tests

```bash
# Run all tests
dotnet test

# Run specific project
dotnet test tests/Authoring.Core.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific category
dotnet test --filter "Category=UI"
```

## Coverage Goals

- Line Coverage: >98%
- Branch Coverage: >96%
- All public APIs should have tests
- Edge cases and error paths should be covered
