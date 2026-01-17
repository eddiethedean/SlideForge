using Avalonia;
using Avalonia.Headless;
using Authoring.Desktop;

[assembly: AvaloniaTestApplication(typeof(Authoring.Desktop.Tests.TestAppBuilder))]

namespace Authoring.Desktop.Tests;

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
