using System.Diagnostics;
using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibility : UiAccessibility
{
    protected readonly UiaAccessibilityIdentity Identity;

    public UiaAccessibility(UiaAccessibilityIdentity uiaAccessibilityIdentity)
    {
        Identity = uiaAccessibilityIdentity ??
                   throw new ArgumentNullException(nameof(uiaAccessibilityIdentity));
        Technology = UiAccessibilityTechnology.Uia;
        Platform = PlatformID.Win32NT;
        Version = new Version(3, 0, 0);
    }

    public override void Record(object element)
    {
        if (element is not AutomationElement automationElement) throw new NotSupportedException(nameof(element));
        var uiaElementPaths = new DistinctStack<UiAccessibilityElement>();
        var currentElement = automationElement;
        FileName = Process.GetProcessById(currentElement.Properties.ProcessId).ProcessName;
        uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        while (currentElement.Parent != null)
        {
            if (currentElement.Parent.Equals(Identity.RootElement))
                break;
            currentElement = Identity.TreeWalker.GetParent(currentElement);
            uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        }

        RecordElements = uiaElementPaths;
    }

    public override void Playback(DistinctStack<UiAccessibilityElement> paths)
    {
        if (!(paths?.Any() ?? false)) throw new InvalidOperationException(nameof(paths));

        //AutomationElement? currentElement = null;
        //while (paths.TryPop(out var item))
        //{
        //    if (item.Element is not AutomationElement automationElement) break;
        //    currentElement = Identity.TreeWalker.GetFirstChild(automationElement);
        //    if (currentElement != null)
        //    {
        //        currentElement.FindFirst(automationElement);
        //    }

        //}
    }
}