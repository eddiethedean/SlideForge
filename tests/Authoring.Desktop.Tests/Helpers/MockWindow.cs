using Avalonia.Controls;

namespace Authoring.Desktop.Tests.Helpers;

/// <summary>
/// Mock window implementation for testing dialogs and window-dependent code.
/// </summary>
public class MockWindow : Window
{
    public MockWindow() : base()
    {
        // Minimal setup for testing
    }

    public bool WasShown { get; private set; }

    public new void Show()
    {
        WasShown = true;
    }

    public new void Hide()
    {
        WasShown = false;
    }
}
