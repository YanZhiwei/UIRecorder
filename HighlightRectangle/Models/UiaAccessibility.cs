using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;

namespace HighlightRectangle.Models;

internal class UiaAccessibility : UiAccessibility
{
    private readonly UIA3Automation _automation = new();
    private readonly IObjectMapper _mapper;
    private readonly AutomationElement _rootElement;
    private readonly ISerializer _serializer;
    private readonly AutomationElement _targetElement;
    private readonly ITreeWalker _treeWalker;

    public UiaAccessibility(AutomationElement automationElement, ISerializer serializer, IObjectMapper mapper)
    {
        _targetElement = automationElement ?? throw new ArgumentNullException(nameof(automationElement));
        _serializer = serializer;
        _mapper = mapper;
        Process = Process.GetProcessById(automationElement.Properties.ProcessId);
        Name = nameof(UiaAccessibility);
        Type = "UIA";
        Version = new Version(3, 0, 0);
        _treeWalker = _automation.TreeWalkerFactory.GetControlViewWalker();
        _rootElement = _automation.GetDesktop();
    }

    public override Stack<UiAccessibilityElement> GetElementStack()
    {
        var uiaElementPaths = new Stack<UiAccessibilityElement>();
        var targetElement = _targetElement;
        uiaElementPaths.Push(DtoAccessibilityElement(targetElement));
        while (targetElement.Parent != null && targetElement.Parent != _rootElement)
        {
            targetElement = _treeWalker.GetParent(targetElement);
            uiaElementPaths.Push(DtoAccessibilityElement(targetElement));
        }

        var json = _serializer.SerializeObject(uiaElementPaths);
        return uiaElementPaths;
    }

    protected override UiAccessibilityElement DtoAccessibilityElement(object element)
    {
        if (element is AutomationElement automationElement)
        {
            var uiAccessibilityElement = new UiAccessibilityElement
            {
                Name = automationElement.Properties.Name.ValueOrDefault,
                ActualWidth = automationElement.ActualWidth,
                ActualHeight = automationElement.ActualHeight,
                BoundingRectangle = new UiAccessibilityElementBoundingRectangle(automationElement.BoundingRectangle),
                Id = automationElement.Properties.AutomationId.ValueOrDefault,
                IsEnabled = automationElement.Properties.IsEnabled.ValueOrDefault,
                IsOffscreen = automationElement.Properties.IsOffscreen.ValueOrDefault,
                IsDialog = automationElement.Properties.IsDialog.ValueOrDefault
            };
            switch (automationElement.GetControlType())
            {
                case ControlType.Document:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Document;
                    break;
                case ControlType.Calendar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Calendar;
                    break;
                case ControlType.SplitButton:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.SplitButton;
                    break;
                case ControlType.List:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.List;
                    break;
                case ControlType.ListItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ListItem;
                    break;
                case ControlType.Thumb:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Thumb;
                    break;
                case ControlType.Custom:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Custom;
                    break;
                case ControlType.Tree:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Tree;
                    break;
                case ControlType.TreeItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.TreeItem;
                    break;
                case ControlType.Button:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Button;
                    break;
                case ControlType.CheckBox:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.CheckBox;
                    break;
                case ControlType.ComboBox:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ComboBox;
                    break;
                case ControlType.Edit:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Edit;
                    break;
                case ControlType.Group:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Group;
                    break;
                case ControlType.Image:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Image;
                    break;
                case ControlType.Menu:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Menu;
                    break;
                case ControlType.MenuBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.MenuBar;
                    break;
                case ControlType.MenuItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.MenuItem;
                    break;
                case ControlType.ProgressBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ProgressBar;
                    break;
                case ControlType.RadioButton:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.RadioButton;
                    break;
                case ControlType.ScrollBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ScrollBar;
                    break;
                case ControlType.Slider:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Slider;
                    break;
                case ControlType.Spinner:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Spinner;
                    break;
                case ControlType.StatusBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.StatusBar;
                    break;
                case ControlType.Tab:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Tab;
                    break;
                case ControlType.TabItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.TabItem;
                    break;
                case ControlType.Table:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Table;
                    break;
                case ControlType.Text:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Text;
                    break;
                case ControlType.ToolBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ToolBar;
                    break;
                case ControlType.ToolTip:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.ToolTip;
                    break;
                case ControlType.Window:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Window;
                    break;
                case ControlType.Separator:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Separator;
                    break;
                case ControlType.SemanticZoom:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.SemanticZoom;
                    break;
                case ControlType.AppBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.AppBar;
                    break;
                case ControlType.TitleBar:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.TitleBar;
                    break;
                case ControlType.Header:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Header;
                    break;
                case ControlType.HeaderItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.HeaderItem;
                    break;
                case ControlType.Hyperlink:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Hyperlink;
                    break;
                case ControlType.DataGrid:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.DataGrid;
                    break;
                case ControlType.DataItem:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.DataItem;
                    break;
                default:
                    uiAccessibilityElement.ControlType = UiAccessibilityControlType.Unknown;
                    break;
            }

            return uiAccessibilityElement;
        }

        return null;
    }
}