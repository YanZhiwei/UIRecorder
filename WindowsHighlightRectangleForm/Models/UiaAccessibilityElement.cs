using FlaUI.Core.AutomationElements;
using Tenon.Helper.Internal;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibilityElement : UiAccessibilityElement, IUiAccessibilityElementReplayActions
{
    public static readonly Dictionary<string, UiaAccessibilityElement> UiaAccessibilityElements;

    static UiaAccessibilityElement()
    {
        UiaAccessibilityElements = ReflectHelper.CreateDerivedInstances<UiaAccessibilityElement>()
            .Where(c => c.Metadata != null)
            .ToDictionary(key => key.Metadata.IdentityString, value => value);
    }

    public IAccessibilityMetadata? Metadata { get; protected set; }

    public virtual void Click()
    {
        var accessibilityElement = GetAccessibilityElement();
        if (accessibilityElement != null)
        {
            accessibilityElement.Click();
        }
        else
        {
            CheckNativeElement(out var automationElement);
            automationElement.Click();
        }
    }

    private UiaAccessibilityElement? GetAccessibilityElement()
    {
        var processName = Accessibility.FileName;
        var findKey =
            UiaAccessibilityElements.Keys.FirstOrDefault(c =>
                c.Contains(processName, StringComparison.OrdinalIgnoreCase));
        return string.IsNullOrWhiteSpace(findKey) ? null : UiaAccessibilityElements[findKey];
    }

    protected void CheckNativeElement(out AutomationElement automationElement)
    {
        if (NativeElement is not AutomationElement uiaElement)
            throw new InvalidOperationException(nameof(Click));
        automationElement = uiaElement;
    }
}