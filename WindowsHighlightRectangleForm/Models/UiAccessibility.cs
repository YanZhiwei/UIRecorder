using System.Diagnostics;

namespace WindowsHighlightRectangleForm.Models;

public abstract class UiAccessibility
{
    public Process Process { get; protected set; }

    public string Name { get; protected set; }

    public string Type { get; protected set; }

    public Version Version { get; protected set; }

    public abstract DistinctStack<UiAccessibilityElement> Record(object element);

    public abstract void Playback();
}