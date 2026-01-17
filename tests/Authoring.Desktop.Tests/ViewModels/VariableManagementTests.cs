using Authoring.Core.Models;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class VariableManagementTests
{
    [Fact]
    public void AddVariable_WithValidData_AddsToProject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test Project" };
        
        var variable = new Variable
        {
            Id = "var1",
            Name = "TestVar",
            Type = VariableType.Boolean,
            DefaultValue = true
        };
        project.AddVariable(variable);

        // Act
        viewModel.CurrentProject = project; // This updates the Variables collection

        // Assert
        Assert.Single(project.Variables);
        Assert.Equal(variable, project.Variables[0]);
        Assert.Single(viewModel.Variables);
        Assert.Equal(variable, viewModel.Variables[0]);
    }

    [Fact]
    public void DeleteVariable_RemovesFromProject()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project = new Project { Name = "Test Project" };
        var variable = new Variable
        {
            Id = "var1",
            Name = "TestVar",
            Type = VariableType.Number,
            DefaultValue = 42
        };
        project.AddVariable(variable);
        viewModel.CurrentProject = project;

        // Act
        viewModel.DeleteVariable(variable);

        // Assert
        Assert.Empty(project.Variables);
        Assert.Empty(viewModel.Variables);
    }

    [Fact]
    public void Variables_WhenProjectChanges_UpdatesCollection()
    {
        // Arrange
        var mockService = new Mock<IProjectService>();
        var viewModel = new MainWindowViewModel(mockService.Object);
        var project1 = new Project { Name = "Project 1" };
        project1.AddVariable(new Variable { Id = "var1", Name = "Var1", Type = VariableType.Boolean, DefaultValue = true });

        var project2 = new Project { Name = "Project 2" };
        project2.AddVariable(new Variable { Id = "var2", Name = "Var2", Type = VariableType.String, DefaultValue = "test" });

        // Act
        viewModel.CurrentProject = project1;
        Assert.Single(viewModel.Variables);

        viewModel.CurrentProject = project2;

        // Assert
        Assert.Single(viewModel.Variables);
        Assert.Equal("Var2", viewModel.Variables[0].Name);
    }

    [Fact]
    public void Variable_BooleanType_HasCorrectDefaultValue()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "bool1",
            Name = "IsEnabled",
            Type = VariableType.Boolean,
            DefaultValue = true
        };

        // Assert
        Assert.Equal(VariableType.Boolean, variable.Type);
        Assert.True((bool)variable.DefaultValue!);
    }

    [Fact]
    public void Variable_NumberType_HasCorrectDefaultValue()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "num1",
            Name = "Counter",
            Type = VariableType.Number,
            DefaultValue = 100
        };

        // Assert
        Assert.Equal(VariableType.Number, variable.Type);
        Assert.Equal(100, variable.DefaultValue);
    }

    [Fact]
    public void Variable_StringType_HasCorrectDefaultValue()
    {
        // Arrange & Act
        var variable = new Variable
        {
            Id = "str1",
            Name = "Message",
            Type = VariableType.String,
            DefaultValue = "Hello World"
        };

        // Assert
        Assert.Equal(VariableType.String, variable.Type);
        Assert.Equal("Hello World", variable.DefaultValue);
    }
}
