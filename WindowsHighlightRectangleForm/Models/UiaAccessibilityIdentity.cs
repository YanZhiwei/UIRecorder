using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Tenon.Helper.Internal;
using Application = FlaUI.Core.Application;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibilityIdentity : UiAccessibilityIdentity
{
    public static readonly Dictionary<string, IUiaAppAccessibilityIdentity> AppAccessibilityIdentities;

    public readonly UIA3Automation Automation = new();
    public readonly AutomationElement RootElement;
    public readonly ITreeWalker TreeWalker;

    static UiaAccessibilityIdentity()
    {
        AppAccessibilityIdentities = ReflectHelper
            .CreateInterfaceTypeInstances<IUiaAppAccessibilityIdentity>()
            .ToDictionary(key => key.IdentityString, value => value);
    }

    public UiaAccessibilityIdentity()
    {
        TreeWalker = Automation.TreeWalkerFactory.GetControlViewWalker();
        RootElement = Automation.GetDesktop();
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
        return new UiAccessibilityElement
        {
            Name = automationElement.Properties.Name.ValueOrDefault,
            ActualWidth = automationElement.ActualWidth,
            ActualHeight = automationElement.ActualHeight,
            BoundingRectangle = automationElement.BoundingRectangle,
            Id = automationElement.Properties.AutomationId.ValueOrDefault,
            IsEnabled = automationElement.Properties.IsEnabled.ValueOrDefault,
            IsOffscreen = automationElement.Properties.IsOffscreen.ValueOrDefault,
            IsDialog = automationElement.Properties.IsDialog.ValueOrDefault,
            Element = automationElement,
            ControlType = automationElement.GetControlType() switch
            {
                ControlType.Document => UiAccessibilityControlType.Document,
                ControlType.Calendar => UiAccessibilityControlType.Calendar,
                ControlType.SplitButton => UiAccessibilityControlType.SplitButton,
                ControlType.List => UiAccessibilityControlType.List,
                ControlType.ListItem => UiAccessibilityControlType.ListItem,
                ControlType.Thumb => UiAccessibilityControlType.Thumb,
                ControlType.Custom => UiAccessibilityControlType.Custom,
                ControlType.Tree => UiAccessibilityControlType.Tree,
                ControlType.TreeItem => UiAccessibilityControlType.TreeItem,
                ControlType.Button => UiAccessibilityControlType.Button,
                ControlType.CheckBox => UiAccessibilityControlType.CheckBox,
                ControlType.ComboBox => UiAccessibilityControlType.ComboBox,
                ControlType.Edit => UiAccessibilityControlType.Edit,
                ControlType.Group => UiAccessibilityControlType.Group,
                ControlType.Image => UiAccessibilityControlType.Image,
                ControlType.Menu => UiAccessibilityControlType.Menu,
                ControlType.MenuBar => UiAccessibilityControlType.MenuBar,
                ControlType.MenuItem => UiAccessibilityControlType.MenuItem,
                ControlType.ProgressBar => UiAccessibilityControlType.ProgressBar,
                ControlType.RadioButton => UiAccessibilityControlType.RadioButton,
                ControlType.ScrollBar => UiAccessibilityControlType.ScrollBar,
                ControlType.Slider => UiAccessibilityControlType.Slider,
                ControlType.Spinner => UiAccessibilityControlType.Spinner,
                ControlType.StatusBar => UiAccessibilityControlType.StatusBar,
                ControlType.Tab => UiAccessibilityControlType.Tab,
                ControlType.TabItem => UiAccessibilityControlType.TabItem,
                ControlType.Table => UiAccessibilityControlType.Table,
                ControlType.Text => UiAccessibilityControlType.Text,
                ControlType.ToolBar => UiAccessibilityControlType.ToolBar,
                ControlType.ToolTip => UiAccessibilityControlType.ToolTip,
                ControlType.Window => UiAccessibilityControlType.Window,
                ControlType.Separator => UiAccessibilityControlType.Separator,
                ControlType.SemanticZoom => UiAccessibilityControlType.SemanticZoom,
                ControlType.AppBar => UiAccessibilityControlType.AppBar,
                ControlType.TitleBar => UiAccessibilityControlType.TitleBar,
                ControlType.Header => UiAccessibilityControlType.Header,
                ControlType.HeaderItem => UiAccessibilityControlType.HeaderItem,
                ControlType.Hyperlink => UiAccessibilityControlType.Hyperlink,
                ControlType.DataGrid => UiAccessibilityControlType.DataGrid,
                ControlType.DataItem => UiAccessibilityControlType.DataItem,
                _ => UiAccessibilityControlType.Unknown
            }
        };
    }
}