using Authoring.Core.Models;

namespace Authoring.Core.Tests.Helpers;

/// <summary>
/// Fluent builder for creating test slides.
/// </summary>
public class SlideBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _title = "Test Slide";
    private double _width = 1920;
    private double _height = 1080;
    private readonly List<Layer> _layers = new();

    public SlideBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public SlideBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public SlideBuilder WithDimensions(double width, double height)
    {
        _width = width;
        _height = height;
        return this;
    }

    public SlideBuilder WithLayer(Layer layer)
    {
        _layers.Add(layer);
        return this;
    }

    public SlideBuilder WithLayer(string id, string name, bool visible = true)
    {
        _layers.Add(new Layer
        {
            Id = id,
            Name = name,
            Visible = visible
        });
        return this;
    }

    public SlideBuilder WithBaseLayer()
    {
        return WithLayer(Guid.NewGuid().ToString(), "Base Layer", true);
    }

    public SlideBuilder WithObject(SlideObject obj)
    {
        if (_layers.Count == 0)
        {
            WithBaseLayer();
        }

        var baseLayer = _layers[0];
        baseLayer.Objects.Add(obj);
        return this;
    }

    public SlideBuilder WithObject(Func<ObjectBuilder, SlideObject> configure)
    {
        var builder = new ObjectBuilder();
        var obj = configure(builder);
        return WithObject(obj);
    }

    public Slide Build()
    {
        var slide = new Slide
        {
            Id = _id,
            Title = _title,
            Width = _width,
            Height = _height
        };

        if (_layers.Count == 0)
        {
            slide.Layers.Add(new Layer
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Base Layer",
                Visible = true
            });
        }
        else
        {
            foreach (var layer in _layers)
            {
                slide.Layers.Add(layer);
            }
        }

        return slide;
    }

    public static SlideBuilder Create() => new();
}
