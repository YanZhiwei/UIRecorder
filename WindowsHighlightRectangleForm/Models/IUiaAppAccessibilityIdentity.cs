using FlaUI.Core;
using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public interface IUiaAppAccessibilityIdentity
{
    public string[] SupportedProcessNames { get; }

    public string IdentityString { get; }

    public AutomationElement? FromHoveredElement( Point location, AutomationElement hoveredElement, ITreeWalker treeWalker);
}