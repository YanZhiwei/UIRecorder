using System.Diagnostics;

namespace UIRecorder.Models;

internal abstract class UiAccessibility
{
    public Process Process { get; protected set; }

    public string Name { get; protected set; }

    public string Type { get; protected set; }

    public Version Version { get; protected set; }

    public abstract Stack<UiAccessibilityElement> GetElementStack();
}