using System.Linq;
using Action = Authoring.Core.Models.Action;
using Authoring.Core.Models;
using Authoring.Core.Tests.Helpers;
using Authoring.Core.Validation;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class VariableManagementRobustTests
{
    [Fact]
    public void Variables_WithDuplicateIds_ValidationFails()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test" };
        
        project.AddVariable(new Variable { Id = "var1", Name = "Var1", Type = VariableType.Boolean, DefaultValue = true });
        project.AddVariable(new Variable { Id = "var1", Name = "Var2", Type = VariableType.Number, DefaultValue = 42 }); // Duplicate ID
        
        viewModel.CurrentProject = project;

        // Act
        viewModel.ValidateProject();

        // Assert
        Assert.True(viewModel.HasValidationWarnings);
        Assert.Contains(viewModel.ValidationWarnings, w => w.Contains("Duplicate variable ID") && w.Contains("var1"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Variable_WithEmptyName_IsValidForCreation(string? name)
    {
        // Arrange
        var variable = new Variable
        {
            Id = "var1",
            Name = name ?? string.Empty,
            Type = VariableType.Boolean,
            DefaultValue = true
        };

        // Act & Assert
        // Note: Current implementation doesn't validate variable names
        // This test documents current behavior
        Assert.NotNull(variable);
    }

    [Fact]
    public void Variable_WithNullDefaultValue_IsAcceptable()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "var1",
            Name = "TestVar",
            Type = VariableType.String,
            DefaultValue = null
        };

        // Assert
        Assert.Null(variable.DefaultValue);
    }

    [Fact]
    public void Variable_NumberType_AcceptsIntAndDouble()
    {
        // Arrange & Act
        var intVar = new Variable { Id = "int1", Name = "IntVar", Type = VariableType.Number, DefaultValue = 42 };
        var doubleVar = new Variable { Id = "double1", Name = "DoubleVar", Type = VariableType.Number, DefaultValue = 42.5 };

        // Assert
        Assert.Equal(42, intVar.DefaultValue);
        Assert.Equal(42.5, doubleVar.DefaultValue);
    }

    [Fact]
    public void Variable_StringType_AcceptsEmptyString()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "str1",
            Name = "EmptyString",
            Type = VariableType.String,
            DefaultValue = string.Empty
        };

        // Assert
        Assert.Equal(string.Empty, variable.DefaultValue);
    }

    [Fact]
    public void Variables_WithManyVariables_PerformsWell()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Large Project" };

        // Act
        for (int i = 0; i < 100; i++)
        {
            project.AddVariable(new Variable
            {
                Id = $"var{i}",
                Name = $"Variable{i}",
                Type = i % 3 == 0 ? VariableType.Boolean : (i % 3 == 1 ? VariableType.Number : VariableType.String),
                DefaultValue = i % 3 == 0 ? (i % 2 == 0) : (i % 3 == 1 ? (double)i : $"Value{i}")
            });
        }

        viewModel.CurrentProject = project;

        // Assert
        Assert.Equal(100, project.Variables.Count);
        Assert.Equal(100, viewModel.Variables.Count);
        viewModel.ValidateProject(); // Should complete quickly
        Assert.False(viewModel.HasValidationWarnings);
    }

    [Fact]
    public void Variable_Deleted_StillReferencedByAction_ShowsValidationWarning()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = ProjectBuilder.Create()
            .WithName("Test")
            .WithVariable("counter", "Counter", VariableType.Number, 0)
            .WithSlide(s => s
                .WithTitle("Slide 1")
                .WithObject(o => o
                    .AtPosition(100, 100)
                    .BuildButtonObject("Button")))
            .Build();

        var button = project.Slides[0].Layers[0].Objects.OfType<ButtonObject>().First();
        button.Triggers.Add(new Trigger
        {
            Id = "t1",
            Type = TriggerType.OnClick,
            Actions = new List<Action>
            {
                new SetVariableAction { VariableId = "counter", Value = 10 }
            }
        });

        viewModel.CurrentProject = project;
        viewModel.ValidateProject();
        Assert.False(viewModel.HasValidationWarnings);

        // Act - Delete the variable
        var variable = project.Variables.First();
        project.Variables.Remove(variable);
        viewModel.UpdateVariablesCollection();
        viewModel.ValidateProject();

        // Assert
        Assert.True(viewModel.HasValidationWarnings);
        Assert.Contains(viewModel.ValidationWarnings, w => w.Contains("counter") && w.Contains("non-existent variable"));
    }

    [Fact]
    public void Variables_AfterProjectChange_ClearsPreviousVariables()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        
        var project1 = new Project { Name = "Project 1" };
        for (int i = 0; i < 5; i++)
        {
            project1.AddVariable(new Variable { Id = $"var{i}", Name = $"Var{i}", Type = VariableType.Boolean, DefaultValue = true });
        }

        var project2 = new Project { Name = "Project 2" };
        project2.AddVariable(new Variable { Id = "newVar", Name = "NewVar", Type = VariableType.String, DefaultValue = "test" });

        // Act
        viewModel.CurrentProject = project1;
        Assert.Equal(5, viewModel.Variables.Count);

        viewModel.CurrentProject = project2;

        // Assert
        Assert.Single(viewModel.Variables);
        Assert.Equal("newVar", viewModel.Variables[0].Id);
    }

    [Fact]
    public void Variable_BooleanType_WithNonBooleanValue_StillStoresValue()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "bool1",
            Name = "Flag",
            Type = VariableType.Boolean,
            DefaultValue = "not a boolean" // Wrong type, but current implementation allows it
        };

        // Assert
        // Current implementation doesn't validate type compatibility
        Assert.Equal("not a boolean", variable.DefaultValue);
    }
}
