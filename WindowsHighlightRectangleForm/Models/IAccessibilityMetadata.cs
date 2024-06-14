namespace WindowsHighlightRectangleForm.Models;

public interface IAccessibilityMetadata
{
    public string[] SupportedProcessNames { get; }

    public string IdentityString { get; }
}