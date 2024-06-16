using Microsoft.Extensions.DependencyInjection;
using Mortise.Accessibility.Abstractions;
using Mortise.UiaAccessibility.Extensions;
using Mortise.UiaAccessibility.WeChat.Configurations;

namespace WindowsHighlightSample;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.

        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var services = new ServiceCollection();

        ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider();
        using (serviceProvider)
        {
            var form1 = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form1);
        }
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddScoped<MainForm>();
        services.AddUiaAccessible(option => { option.AddWeChatAccessible(); });
    }
}