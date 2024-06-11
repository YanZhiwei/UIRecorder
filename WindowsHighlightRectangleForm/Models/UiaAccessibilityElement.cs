using FlaUI.Core.AutomationElements;
using FlaUI.UIA3.Patterns;

namespace WindowsHighlightRectangleForm.Models;

public sealed class UiaAccessibilityElement : UiAccessibilityElement, IUiAccessibilityElementReplayActions
{
    public void Click()
    {
        if (this.NativeElement is not AutomationElement automationElement)
            throw new InvalidOperationException(nameof(Click));
        automationElement.Click();
    }
}