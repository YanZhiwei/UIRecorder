using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public sealed class WeChatUiaAccessibilityIdentity : UiaAccessibilityIdentity
{
    public WeChatUiaAccessibilityIdentity()
    {
        ProcessName = "WeChat";
    }

    public override AutomationElement? FromPoint(int x, int y)
    {
        var hoveredElement = GetHoveredElement(x, y);
        if (hoveredElement is not AutomationElement automationElement) return null;
        TreeWalker.GetParent(automationElement);
        automationElement = automationElement.FindChildDescendants(new Point(x, y)) ?? automationElement;
        return automationElement;
        //return DtoAccessibilityElement(automationElement);
    }
}