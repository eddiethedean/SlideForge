using Authoring.Core.Models;
using Xunit;

namespace Authoring.Core.Tests.Extensions;

/// <summary>
/// Custom assertion extensions for domain models.
/// </summary>
public static class AssertExtensions
{
    /// <summary>
    /// Asserts that two projects are equal in structure (ignoring timestamps and IDs if not specified).
    /// </summary>
    public static void AssertProjectsEqual(Project expected, Project actual, bool compareIds = true, bool compareTimestamps = false)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Author, actual.Author);
        Assert.Equal(expected.Version, actual.Version);

        if (compareTimestamps)
        {
            Assert.Equal(expected.CreatedAt, actual.CreatedAt);
            Assert.Equal(expected.ModifiedAt, actual.ModifiedAt);
        }

        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Variables.Count, actual.Variables.Count);
        Assert.Equal(expected.Slides.Count, actual.Slides.Count);

        for (int i = 0; i < expected.Variables.Count; i++)
        {
            AssertVariablesEqual(expected.Variables[i], actual.Variables[i], compareIds);
        }

        for (int i = 0; i < expected.Slides.Count; i++)
        {
            AssertSlidesEqual(expected.Slides[i], actual.Slides[i], compareIds);
        }
    }

    /// <summary>
    /// Asserts that two slides are equal in structure.
    /// </summary>
    public static void AssertSlidesEqual(Slide expected, Slide actual, bool compareIds = true)
    {
        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Title, actual.Title);
        Assert.Equal(expected.Width, actual.Width);
        Assert.Equal(expected.Height, actual.Height);
        Assert.Equal(expected.Layers.Count, actual.Layers.Count);

        for (int i = 0; i < expected.Layers.Count; i++)
        {
            AssertLayersEqual(expected.Layers[i], actual.Layers[i], compareIds);
        }
    }

    /// <summary>
    /// Asserts that two layers are equal in structure.
    /// </summary>
    public static void AssertLayersEqual(Layer expected, Layer actual, bool compareIds = true)
    {
        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Visible, actual.Visible);
        Assert.Equal(expected.Objects.Count, actual.Objects.Count);

        for (int i = 0; i < expected.Objects.Count; i++)
        {
            AssertSlideObjectsEqual(expected.Objects[i], actual.Objects[i], compareIds);
        }
    }

    /// <summary>
    /// Asserts that two slide objects are equal in structure.
    /// </summary>
    public static void AssertSlideObjectsEqual(SlideObject expected, SlideObject actual, bool compareIds = true)
    {
        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.X, actual.X);
        Assert.Equal(expected.Y, actual.Y);
        Assert.Equal(expected.Width, actual.Width);
        Assert.Equal(expected.Height, actual.Height);
        Assert.Equal(expected.Visible, actual.Visible);

        Assert.Equal(expected.GetType(), actual.GetType());

        switch (expected)
        {
            case TextObject expectedText when actual is TextObject actualText:
                Assert.Equal(expectedText.Text, actualText.Text);
                Assert.Equal(expectedText.FontSize, actualText.FontSize);
                Assert.Equal(expectedText.Color, actualText.Color);
                break;

            case ImageObject expectedImage when actual is ImageObject actualImage:
                Assert.Equal(expectedImage.SourcePath, actualImage.SourcePath);
                Assert.Equal(expectedImage.MaintainAspectRatio, actualImage.MaintainAspectRatio);
                break;

            case ButtonObject expectedButton when actual is ButtonObject actualButton:
                Assert.Equal(expectedButton.Label, actualButton.Label);
                Assert.Equal(expectedButton.Enabled, actualButton.Enabled);
                break;
        }

        // Compare timelines
        if (expected.Timeline == null)
        {
            Assert.Null(actual.Timeline);
        }
        else
        {
            Assert.NotNull(actual.Timeline);
            Assert.Equal(expected.Timeline.StartTime, actual.Timeline!.StartTime);
            Assert.Equal(expected.Timeline.Duration, actual.Timeline.Duration);
        }

        // Compare triggers
        Assert.Equal(expected.Triggers.Count, actual.Triggers.Count);
        for (int i = 0; i < expected.Triggers.Count; i++)
        {
            AssertTriggersEqual(expected.Triggers[i], actual.Triggers[i], compareIds);
        }
    }

    /// <summary>
    /// Asserts that two variables are equal in structure.
    /// </summary>
    public static void AssertVariablesEqual(Variable expected, Variable actual, bool compareIds = true)
    {
        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Type, actual.Type);
        Assert.Equal(expected.DefaultValue, actual.DefaultValue);
    }

    /// <summary>
    /// Asserts that two triggers are equal in structure.
    /// </summary>
    public static void AssertTriggersEqual(Trigger expected, Trigger actual, bool compareIds = true)
    {
        if (compareIds)
        {
            Assert.Equal(expected.Id, actual.Id);
        }

        Assert.Equal(expected.Type, actual.Type);
        Assert.Equal(expected.ObjectId, actual.ObjectId);
        Assert.Equal(expected.Actions.Count, actual.Actions.Count);

        for (int i = 0; i < expected.Actions.Count; i++)
        {
            AssertActionsEqual(expected.Actions[i], actual.Actions[i]);
        }
    }

    /// <summary>
    /// Asserts that two actions are equal in structure.
    /// </summary>
    public static void AssertActionsEqual(Authoring.Core.Models.Action expected, Authoring.Core.Models.Action actual)
    {
        Assert.Equal(expected.GetType(), actual.GetType());

        switch (expected)
        {
            case NavigateToSlideAction expectedNav when actual is NavigateToSlideAction actualNav:
                Assert.Equal(expectedNav.TargetSlideId, actualNav.TargetSlideId);
                break;

            case SetVariableAction expectedSet when actual is SetVariableAction actualSet:
                Assert.Equal(expectedSet.VariableId, actualSet.VariableId);
                Assert.Equal(expectedSet.Value, actualSet.Value);
                break;

            case ShowLayerAction expectedShow when actual is ShowLayerAction actualShow:
                Assert.Equal(expectedShow.LayerId, actualShow.LayerId);
                break;

            case HideLayerAction expectedHide when actual is HideLayerAction actualHide:
                Assert.Equal(expectedHide.LayerId, actualHide.LayerId);
                break;
        }
    }

    /// <summary>
    /// Asserts that a project is valid (passes all validation rules).
    /// </summary>
    public static void AssertProjectIsValid(Project project)
    {
        var errors = Authoring.Core.Validation.ProjectValidator.ValidateProject(project);
        Assert.Empty(errors);
    }

    /// <summary>
    /// Asserts that a project has validation errors.
    /// </summary>
    public static void AssertProjectHasErrors(Project project, int? expectedErrorCount = null)
    {
        var errors = Authoring.Core.Validation.ProjectValidator.ValidateProject(project);
        Assert.NotEmpty(errors);
        if (expectedErrorCount.HasValue)
        {
            Assert.Equal(expectedErrorCount.Value, errors.Count);
        }
    }
}
