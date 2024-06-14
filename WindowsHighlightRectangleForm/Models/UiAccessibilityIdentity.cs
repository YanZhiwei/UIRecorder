namespace WindowsHighlightRectangleForm.Models;

public abstract class UiAccessibilityIdentity
{
    public string ProcessName { get; protected set; } = "*";
    public UiAccessibilityIdentityPriority Priority { get; protected set; } = UiAccessibilityIdentityPriority.Normal;
    public abstract UiAccessibilityElement? FromPoint(Point location);
    public abstract UiAccessibilityElement? DtoAccessibilityElement(object element, UiAccessibility? accessibility = null);
}