using AutoMapper;
using FlaUI.Core.Definitions;
using WindowsHighlightRectangleForm.Models;

namespace WindowsHighlightRectangleForm;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<ControlType, UiAccessibilityControlType>().ConvertUsing((value, destination) =>
        {
            switch (value)
            {
                case ControlType.AppBar:
                    return UiAccessibilityControlType.AppBar;
                case ControlType.Button:
                    return UiAccessibilityControlType.Button;
                case ControlType.Calendar:
                    return UiAccessibilityControlType.Calendar;
                case ControlType.CheckBox:
                    return UiAccessibilityControlType.CheckBox;
                case ControlType.ComboBox:
                    return UiAccessibilityControlType.ComboBox;
                case ControlType.Custom:
                    return UiAccessibilityControlType.Custom;
                case ControlType.Edit:
                    return UiAccessibilityControlType.Edit;
                case ControlType.DataGrid:
                    return UiAccessibilityControlType.DataGrid;
                case ControlType.Document:
                    return UiAccessibilityControlType.Document;
                case ControlType.Group:
                    return UiAccessibilityControlType.Group;
                case ControlType.Header:
                    return UiAccessibilityControlType.Header;
                case ControlType.Image:
                    return UiAccessibilityControlType.Image;
                case ControlType.List:
                    return UiAccessibilityControlType.List;
                case ControlType.ListItem:
                    return UiAccessibilityControlType.ListItem;
                case ControlType.Menu:
                    return UiAccessibilityControlType.Menu;
                case ControlType.MenuBar:
                    return UiAccessibilityControlType.MenuBar;
                case ControlType.MenuItem:
                    return UiAccessibilityControlType.MenuItem;
                case ControlType.Pane:
                    return UiAccessibilityControlType.Pane;
                case ControlType.ProgressBar:
                    return UiAccessibilityControlType.ProgressBar;
                case ControlType.RadioButton:
                    return UiAccessibilityControlType.RadioButton;
                case ControlType.ScrollBar:
                    return UiAccessibilityControlType.ScrollBar;
                case ControlType.Slider:
                    return UiAccessibilityControlType.Slider;
                case ControlType.Spinner:
                    return UiAccessibilityControlType.Spinner;
                case ControlType.StatusBar:
                    return UiAccessibilityControlType.StatusBar;
                case ControlType.Tab:
                    return UiAccessibilityControlType.Tab;
                case ControlType.TabItem:
                    return UiAccessibilityControlType.TabItem;
                case ControlType.Table:
                    return UiAccessibilityControlType.Table;
                case ControlType.Text:
                    return UiAccessibilityControlType.Text;
                case ControlType.TitleBar:
                    return UiAccessibilityControlType.TitleBar;
                case ControlType.ToolBar:
                    return UiAccessibilityControlType.ToolBar;
                case ControlType.ToolTip:
                    return UiAccessibilityControlType.ToolTip;
                case ControlType.Tree:
                    return UiAccessibilityControlType.Tree;
                case ControlType.TreeItem:
                    return UiAccessibilityControlType.TreeItem;
                case ControlType.Window:
                    return UiAccessibilityControlType.Window;
                case ControlType.Separator:
                    return UiAccessibilityControlType.Separator;
                case ControlType.SemanticZoom:
                    return UiAccessibilityControlType.SemanticZoom;
                case ControlType.Thumb:
                    return UiAccessibilityControlType.Thumb;
                case ControlType.HeaderItem:
                    return UiAccessibilityControlType.HeaderItem;
                case ControlType.Hyperlink:
                    return UiAccessibilityControlType.Hyperlink;
                case ControlType.SplitButton:
                    return UiAccessibilityControlType.SplitButton;
                default:
                    return UiAccessibilityControlType.Unknown;
            }
        });

        CreateMap<UiAccessibilityControlType, ControlType>().ConvertUsing((value, destination) =>
        {
            switch (value)
            {
                case UiAccessibilityControlType.AppBar:
                    return ControlType.AppBar;
                case UiAccessibilityControlType.Button:
                    return ControlType.Button;
                case UiAccessibilityControlType.Calendar:
                    return ControlType.Calendar;
                case UiAccessibilityControlType.CheckBox:
                    return ControlType.CheckBox;
                case UiAccessibilityControlType.ComboBox:
                    return ControlType.ComboBox;
                case UiAccessibilityControlType.Custom:
                    return ControlType.Custom;
                case UiAccessibilityControlType.Edit:
                    return ControlType.Edit;
                case UiAccessibilityControlType.DataGrid:
                    return ControlType.DataGrid;
                case UiAccessibilityControlType.Document:
                    return ControlType.Document;
                case UiAccessibilityControlType.Group:
                    return ControlType.Group;
                case UiAccessibilityControlType.Header:
                    return ControlType.Header;
                case UiAccessibilityControlType.Image:
                    return ControlType.Image;
                case UiAccessibilityControlType.List:
                    return ControlType.List;
                case UiAccessibilityControlType.ListItem:
                    return ControlType.ListItem;
                case UiAccessibilityControlType.Menu:
                    return ControlType.Menu;
                case UiAccessibilityControlType.MenuBar:
                    return ControlType.MenuBar;
                case UiAccessibilityControlType.MenuItem:
                    return ControlType.MenuItem;
                case UiAccessibilityControlType.Pane:
                    return ControlType.Pane;
                case UiAccessibilityControlType.ProgressBar:
                    return ControlType.ProgressBar;
                case UiAccessibilityControlType.RadioButton:
                    return ControlType.RadioButton;
                case UiAccessibilityControlType.ScrollBar:
                    return ControlType.ScrollBar;
                case UiAccessibilityControlType.Slider:
                    return ControlType.Slider;
                case UiAccessibilityControlType.Spinner:
                    return ControlType.Spinner;
                case UiAccessibilityControlType.StatusBar:
                    return ControlType.StatusBar;
                case UiAccessibilityControlType.Tab:
                    return ControlType.Tab;
                case UiAccessibilityControlType.TabItem:
                    return ControlType.TabItem;
                case UiAccessibilityControlType.Table:
                    return ControlType.Table;
                case UiAccessibilityControlType.Text:
                    return ControlType.Text;
                case UiAccessibilityControlType.TitleBar:
                    return ControlType.TitleBar;
                case UiAccessibilityControlType.ToolBar:
                    return ControlType.ToolBar;
                case UiAccessibilityControlType.ToolTip:
                    return ControlType.ToolTip;
                case UiAccessibilityControlType.Tree:
                    return ControlType.Tree;
                case UiAccessibilityControlType.TreeItem:
                    return ControlType.TreeItem;
                case UiAccessibilityControlType.Window:
                    return ControlType.Window;
                case UiAccessibilityControlType.Separator:
                    return ControlType.Separator;
                case UiAccessibilityControlType.SemanticZoom:
                    return ControlType.SemanticZoom;
                case UiAccessibilityControlType.Thumb:
                    return ControlType.Thumb;
                case UiAccessibilityControlType.HeaderItem:
                    return ControlType.HeaderItem;
                case UiAccessibilityControlType.Hyperlink:
                    return ControlType.Hyperlink;
                case UiAccessibilityControlType.SplitButton:
                    return ControlType.SplitButton;
                default:
                    return ControlType.Unknown;
            }
        });
    }
}