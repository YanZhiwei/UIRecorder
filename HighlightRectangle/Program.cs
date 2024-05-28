using Microsoft.Extensions.DependencyInjection;
using Tenon.Mapper.AutoMapper.Extensions;
using Tenon.Serialization.Json.Extensions;

namespace HighlightRectangle;

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

        using (var serviceProvider = services.BuildServiceProvider())
        {
            var form1 = serviceProvider.GetRequiredService<MainForm>();
            Application.Run(form1);
        }
    }

    private static void ConfigureServices(ServiceCollection services)
    {
        services.AddAutoMapperSetup(typeof(AutoMapperProfile).Assembly);
        services.AddSystemTextJsonSerializer();
        services.AddScoped<MainForm>();
    }
}