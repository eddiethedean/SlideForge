using Authoring.Core.Models;

namespace Authoring.Core.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test slide objects.
/// </summary>
public class ObjectBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _name = "Test Object";
    private double _x = 0;
    private double _y = 0;
    private double _width = 100;
    private double _height = 50;
    private bool _visible = true;
    private Timeline? _timeline;

    public ObjectBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ObjectBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ObjectBuilder AtPosition(double x, double y)
    {
        _x = x;
        _y = y;
        return this;
    }

    public ObjectBuilder WithSize(double width, double height)
    {
        _width = width;
        _height = height;
        return this;
    }

    public ObjectBuilder WithVisibility(bool visible)
    {
        _visible = visible;
        return this;
    }

    public ObjectBuilder WithTimeline(double startTime, double duration)
    {
        _timeline = new Timeline
        {
            StartTime = startTime,
            Duration = duration
        };
        return this;
    }

    public TextObject BuildTextObject(string? text = null, double? fontSize = null, string? color = null)
    {
        return new TextObject
        {
            Id = _id,
            Name = _name,
            X = _x,
            Y = _y,
            Width = _width,
            Height = _height,
            Visible = _visible,
            Timeline = _timeline,
            Text = text ?? "Test Text",
            FontSize = fontSize ?? 16,
            Color = color ?? "#000000"
        };
    }

    public ImageObject BuildImageObject(string? sourcePath = null, bool? maintainAspectRatio = null)
    {
        return new ImageObject
        {
            Id = _id,
            Name = _name,
            X = _x,
            Y = _y,
            Width = _width,
            Height = _height,
            Visible = _visible,
            Timeline = _timeline,
            SourcePath = sourcePath ?? "",
            MaintainAspectRatio = maintainAspectRatio ?? true
        };
    }

    public ButtonObject BuildButtonObject(string? label = null, bool? enabled = null)
    {
        return new ButtonObject
        {
            Id = _id,
            Name = _name,
            X = _x,
            Y = _y,
            Width = _width,
            Height = _height,
            Visible = _visible,
            Timeline = _timeline,
            Label = label ?? "Button",
            Enabled = enabled ?? true
        };
    }

    public SlideObject Build()
    {
        // Default to TextObject
        return BuildTextObject();
    }

    public static ObjectBuilder Create() => new();
}
