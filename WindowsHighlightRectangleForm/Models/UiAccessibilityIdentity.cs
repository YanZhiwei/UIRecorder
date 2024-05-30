using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public abstract class UiAccessibilityIdentity
{
    public string ProcessName { get; protected set; } = "*";
    public UiAccessibilityIdentityPriority Priority { get; protected set; } = UiAccessibilityIdentityPriority.Normal;
    public abstract AutomationElement? FromPoint(int x, int y);
    public abstract object? GetHoveredElement(int x, int y);
    public abstract AutomationElement? DtoAccessibilityElement(object element);
}