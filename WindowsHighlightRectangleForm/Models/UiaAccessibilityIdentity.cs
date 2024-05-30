using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibilityIdentity : UiAccessibilityIdentity
{
    protected readonly UIA3Automation Automation = new();
    protected readonly AutomationElement RootElement;
    protected readonly ITreeWalker TreeWalker;

    public UiaAccessibilityIdentity()
    {
        TreeWalker = Automation.TreeWalkerFactory.GetControlViewWalker();
        RootElement = Automation.GetDesktop();
        Priority = UiAccessibilityIdentityPriority.Highest;
    }

    public override AutomationElement? FromPoint(int x, int y)
    {
        var hoveredElement = GetHoveredElement(x, y);
        if (hoveredElement is not AutomationElement automationElement) return null;
        return automationElement;
    }

    public override object? GetHoveredElement(int x, int y)
    {
        var location = new Point(x, x);
        if (location.IsEmpty) return null;
        var hoveredElement =
            Automation.FromPoint(location);
        return hoveredElement;
    }

    public override AutomationElement? DtoAccessibilityElement(object element)
    {
        if (element is not AutomationElement automationElement) return null;
        var uiAccessibilityElement = new UiAccessibilityElement
        {
            Name = automationElement.Properties.Name.ValueOrDefault,
            ActualWidth = automationElement.ActualWidth,
            ActualHeight = automationElement.ActualHeight,
            BoundingRectangle = automationElement.BoundingRectangle,
            Id = automationElement.Properties.AutomationId.ValueOrDefault,
            IsEnabled = automationElement.Properties.IsEnabled.ValueOrDefault,
            IsOffscreen = automationElement.Properties.IsOffscreen.ValueOrDefault,
            IsDialog = automationElement.Properties.IsDialog.ValueOrDefault,
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

        return null;
    }
}