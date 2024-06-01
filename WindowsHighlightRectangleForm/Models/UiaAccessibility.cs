using FlaUI.Core.AutomationElements;
using Tenon.Serialization.Abstractions;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibility : UiAccessibility
{
    protected readonly UiaAccessibilityIdentity Identity;
    protected readonly ISerializer Serializer;

    public UiaAccessibility(ISerializer serializer, UiaAccessibilityIdentity uiaAccessibilityIdentity)
    {
        Serializer = serializer;
        Identity = uiaAccessibilityIdentity ??
                   throw new ArgumentNullException(nameof(uiaAccessibilityIdentity));
        Name = nameof(UiaAccessibility);
        Type = "UIA";
        Version = new Version(3, 0, 0);
    }

    public override DistinctStack<UiAccessibilityElement> Record(object element)
    {
        if (element is not AutomationElement automationElement) throw new NotSupportedException(nameof(element));
        var uiaElementPaths = new DistinctStack<UiAccessibilityElement>();
        var currentElement = automationElement;
        uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement));
        while (currentElement.Parent != null)
        {
            if (currentElement.Parent.Equals(Identity.RootElement))
                break;
            currentElement = Identity.TreeWalker.GetParent(currentElement);
            var standardElement = Identity.DtoAccessibilityElement(currentElement);
            uiaElementPaths.Push(standardElement);
        }

        var json = Serializer.SerializeObject(uiaElementPaths);
        return uiaElementPaths;
    }

    public override void Playback()
    {
        
    }
}