using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Tenon.Helper.Internal;
using Tenon.Mapper.Abstractions;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibilityIdentity : UiAccessibilityIdentity
{
    public static readonly Dictionary<string, IUiaAppAccessibilityIdentity> AppAccessibilityIdentities;

    public readonly UIA3Automation Automation = new();
    public readonly AutomationElement DesktopElement;
    protected readonly IObjectMapper Mapper;
    public readonly ITreeWalker TreeWalker;

    static UiaAccessibilityIdentity()
    {
        AppAccessibilityIdentities = ReflectHelper
            .CreateInterfaceTypeInstances<IUiaAppAccessibilityIdentity>()
            .ToDictionary(key => key.IdentityString, value => value);
    }

    public UiaAccessibilityIdentity(IObjectMapper mapper)
    {
        Mapper = mapper;
        TreeWalker = Automation.TreeWalkerFactory.GetControlViewWalker();
        DesktopElement = Automation.GetDesktop();
        Priority = UiAccessibilityIdentityPriority.Highest;
    }

    public override UiAccessibilityElement? FromPoint(Point location)
    {
        var hoveredElement =
            Automation.FromPoint(location);
        if (hoveredElement == null) return null;
        TreeWalker.GetParent(hoveredElement);
        var processName = Process.GetProcessById(hoveredElement.Properties.ProcessId).ProcessName;
        var findKey =
            AppAccessibilityIdentities.Keys.FirstOrDefault(c =>
                c.Contains(processName, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(findKey))
            hoveredElement = AppAccessibilityIdentities[findKey]
                .FromHoveredElement(location, hoveredElement, TreeWalker);
        return DtoAccessibilityElement(hoveredElement);
    }

    public override UiAccessibilityElement? DtoAccessibilityElement(object element)
    {
        if (element is not AutomationElement automationElement) return null;
        return new UiaAccessibilityElement
        {
            Name = automationElement.Properties.Name.ValueOrDefault,
            ActualWidth = automationElement.ActualWidth,
            ActualHeight = automationElement.ActualHeight,
            BoundingRectangle = automationElement.BoundingRectangle,
            Id = automationElement.Properties.AutomationId.ValueOrDefault,
            IsEnabled = automationElement.Properties.IsEnabled.ValueOrDefault,
            IsOffscreen = automationElement.Properties.IsOffscreen.ValueOrDefault,
            IsDialog = automationElement.Properties.IsDialog.ValueOrDefault,
            NativeElement = automationElement,
            ControlType = Mapper.Map<UiAccessibilityControlType>(automationElement.ControlType)
        };
    }
}