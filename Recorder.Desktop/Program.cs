using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Mortise.Accessibility.Locator.Json.Extensions;
using Mortise.UiaAccessibility.Converters;
using Mortise.UiaAccessibility.Extensions;
using Mortise.UiaAccessibility.WeChat.Configurations;

namespace Recorder.Desktop;

internal class Program
{
    public static IServiceProvider ServiceProvider { get; set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        ServiceProvider = ConfigureServices();
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }


    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddUiaAccessible(option => { option.AddWeChatAccessible(); });
        services.AddJsonLocator(option => { option.UseLocalStorage(); }, [new UiaAccessibleComponentConverter()]);
        return services.BuildServiceProvider();
    }
}