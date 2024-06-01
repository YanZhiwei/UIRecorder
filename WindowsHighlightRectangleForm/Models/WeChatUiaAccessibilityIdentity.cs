using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace WindowsHighlightRectangleForm.Models;

public sealed class WeChatUiaAccessibilityIdentity : IUiaAppAccessibilityIdentity
{
    public WeChatUiaAccessibilityIdentity()
    {
        SupportedProcessNames = ["WeChat", "WeChatApp"];
        IdentityString = string.Join(",", SupportedProcessNames);
    }

    public string[] SupportedProcessNames { get; }
    public string IdentityString { get; }

    public AutomationElement? FromHoveredElement(Point location, AutomationElement hoveredElement,
        ITreeWalker treeWalker)
    {
        return FindChildDescendants(hoveredElement, location);
    }

    private AutomationElement? FindChildDescendants(AutomationElement parent, Point location)
    {
        // 获取所有子元素
        var elementCollection = parent.FindAllChildren(); //parent.GetChildren(); //
        foreach (var element in elementCollection)
            if (element.BoundingRectangle.Contains(location))
            {
                // 递归查找子元素
                var identifyElement = FindChildDescendants(element, location);
                if (identifyElement != null)
                    return identifyElement;

                // 检查控件类型
                var innerControlType = element.ControlType;
                if (innerControlType != ControlType.Pane &&
                    innerControlType != ControlType.Window &&
                    innerControlType != ControlType.Custom)
                    return element;
            }

        return null;
    }
}