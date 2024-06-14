using FlaUI.Core;
using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public interface IUiaAccessibilityIdentity
{
    public IAccessibilityMetadata Metadata { get; }

    public AutomationElement? FromHoveredElement(Point location, AutomationElement hoveredElement,
        ITreeWalker treeWalker);
}