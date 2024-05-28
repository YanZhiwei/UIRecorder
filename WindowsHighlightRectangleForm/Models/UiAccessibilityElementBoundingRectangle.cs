namespace WindowsHighlightRectangleForm.Models;

public struct UiAccessibilityElementBoundingRectangle(Rectangle rect)
{
    public int X { get; private set; } = rect.X;
    public int Y { get; private set; } = rect.Y;
    public int Width { get; private set; } = rect.Width;
    public int Height { get; private set; } = rect.Height;
}