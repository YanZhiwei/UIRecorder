using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3.Identifiers;
using Tenon.Mapper.Abstractions;
using Tenon.Windows.Extensions;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibility : UiAccessibility
{
    protected readonly UiaAccessibilityIdentity Identity;
    protected readonly IObjectMapper Mapper;

    public UiaAccessibility(UiaAccessibilityIdentity uiaAccessibilityIdentity, IObjectMapper mapper)
    {
        Identity = uiaAccessibilityIdentity ??
                   throw new ArgumentNullException(nameof(uiaAccessibilityIdentity));
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Technology = UiAccessibilityTechnology.Uia;
        Platform = PlatformID.Win32NT;
        Version = new Version(3, 0, 0);
    }

    protected IntPtr WindowHandle { get; set; }

    protected virtual AutomationElement GetWindowElement()
    {
        var process = Process.GetProcessById(ProcessId);
        var mainWindowHandle = process?.MainWindowHandle ?? WindowHandle;
        if (mainWindowHandle == IntPtr.Zero)
            mainWindowHandle = process.FindFirstCoreWindows();

        return mainWindowHandle == IntPtr.Zero
            ? Identity.DesktopElement
            : Identity.Automation.FromHandle(mainWindowHandle);
    }

    public override void Record(object element)
    {
        if (element is not AutomationElement automationElement) throw new NotSupportedException(nameof(element));
        ProcessId = automationElement.Properties.ProcessId;
        var uiaElementPaths = new DistinctStack<UiAccessibilityElement>();
        var currentElement = automationElement;
        FileName = Process.GetProcessById(currentElement.Properties.ProcessId).ProcessName;
        uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        while (currentElement.Parent != null)
        {
            if (currentElement.Parent.Equals(Identity.DesktopElement))
                break;
            currentElement = Identity.TreeWalker.GetParent(currentElement);
            uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        }

        RecordElements = uiaElementPaths;
    }

    public override UiAccessibilityElement? FindElement()
    {
        AutomationElement? foundElement = null;
        var parentElement = GetWindowElement();
        while (RecordElements.TryPop(out var item))
        {
            if (item.Element is not AutomationElement automationElement) break;
            if (parentElement == null) continue;
            var condition = CreateCondition(item);
            foundElement = parentElement.FindFirstDescendant(condition);
            parentElement = foundElement ?? Identity.TreeWalker.GetNextSibling(parentElement);
        }

        return foundElement != null ? Identity.DtoAccessibilityElement(foundElement) : null;
    }

    protected virtual ConditionBase CreateCondition(UiAccessibilityElement element)
    {
        var conditions = new List<ConditionBase>();
        if (!string.IsNullOrEmpty(element.Id))
            conditions.Add(new PropertyCondition(AutomationObjectIds.AutomationIdProperty, element.Id));
        if (!string.IsNullOrEmpty(element.Name))
            conditions.Add(new PropertyCondition(AutomationObjectIds.NameProperty, element.Name));
        conditions.Add(new PropertyCondition(AutomationObjectIds.IsDialogProperty, element.IsDialog));
        if (!string.IsNullOrEmpty(element.Id))
            conditions.Add(new PropertyCondition(AutomationObjectIds.AutomationIdProperty, element.Id));
        conditions.Add(new PropertyCondition(AutomationObjectIds.ControlTypeProperty,
            Mapper.Map<ControlType>(element.ControlType)));
        return new AndCondition(conditions.ToArray());
    }
}