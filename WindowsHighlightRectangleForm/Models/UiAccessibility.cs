using System.Diagnostics;

namespace WindowsHighlightRectangleForm.Models;

internal abstract class UiAccessibility
{
    public Process Process { get; protected set; }

    public string Name { get; protected set; }

    public string Type { get; protected set; }

    public Version Version { get; protected set; }

    public abstract DistinctStack<UiAccessibilityElement> GetElementStack();

    public UiAccessibilityElement[] Child { get; protected set; }

    protected abstract UiAccessibilityElement DtoAccessibilityElement(object element);
}