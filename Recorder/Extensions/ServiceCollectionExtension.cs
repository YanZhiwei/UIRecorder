using Microsoft.Extensions.DependencyInjection;
using Mortise.Accessibility.Locator.Json.Extensions;
using Mortise.UiaAccessibility.Converters;
using Mortise.UiaAccessibility.Extensions;
using Mortise.UiaAccessibility.WeChat.Configurations;
using Recorder.ViewModels;

namespace Recorder.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddAccessibleServices(this IServiceCollection services)
    {
        services.AddUiaAccessible(option => { option.AddWeChatAccessible(); });
        services.AddJsonLocator(option => { option.UseLocalStorage(); }, [new UiaAccessibleComponentConverter()]);
    }

    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddTransient<MainWindowViewModel>();
    }
}