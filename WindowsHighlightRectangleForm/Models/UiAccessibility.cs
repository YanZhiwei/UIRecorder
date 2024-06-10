using System.Diagnostics;
using System.Text.Json.Serialization;

namespace WindowsHighlightRectangleForm.Models;

public abstract class UiAccessibility : IDisposable
{
    public string FileName { get; protected set; }
    public int ProcessId { get; protected set; }
    public UiAccessibilityTechnology Technology { get; protected set; }
    public PlatformID Platform { get; set; }
    public Version Version { get; protected set; }

    [JsonPropertyName("paths")] public DistinctStack<UiAccessibilityElement> RecordElements { get; protected set; }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public abstract void Record(object element);
    public abstract UiAccessibilityElement? FindElement(string locatorPath);

    public virtual Process AttachOrLaunch(ProcessStartInfo? startInfo = null)
    {
        var process = Process.GetProcessesByName(FileName);
        if (process?.Any() ?? false) return process.First();

        startInfo ??= new ProcessStartInfo
        {
            FileName = FileName
        };

        return Process.Start(startInfo);
    }

    public virtual void Launch()
    {
    }

    public virtual void Attach()
    {
    }

    public virtual void Close()
    {
    }
}