namespace HighlightRectangle.Models;

internal class UiAccessibilityElement
{
    //public AutomationElement[] Child { get; set; }

    public string Name { get; set; }

    public UiAccessibilityControlType ControlType { get; set; }

    public UiAccessibilityElementBoundingRectangle BoundingRectangle { get; set; }

    public bool IsOffscreen { get; set; }

    public bool IsEnabled { get; set; }

    public double ActualHeight { get; set; }

    public double ActualWidth { get; set; }

    public bool IsDialog { get; set; }

    public string Id { get; set; }
}