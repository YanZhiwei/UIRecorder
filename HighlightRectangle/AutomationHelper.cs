using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;

namespace HighlightRectangle;

public static class AutomationHelper
{
    public static AutomationElement? FindChildDescendants(this AutomationElement parent, Point location)
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

    public static IEnumerable<AutomationElement> GetChildren(this AutomationElement element, bool reverse = false)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        if (!reverse)
            foreach (var e in element.FindAllChildren(TrueCondition.Default))
                yield return e;
    }

    public static ControlType GetControlType(this AutomationElement element)
    {
        var controlType = element.ControlType;
        if (controlType != ControlType.Unknown) return controlType;
        var className = element.ClassName;
        if (className.Equals("ApplicationBar", StringComparison.OrdinalIgnoreCase)) return ControlType.AppBar;

        return className.Equals("SemanticZoom", StringComparison.OrdinalIgnoreCase)
            ? ControlType.SemanticZoom
            : ControlType.Custom;
    }
}