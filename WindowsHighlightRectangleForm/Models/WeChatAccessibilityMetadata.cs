namespace WindowsHighlightRectangleForm.Models;

public sealed class WeChatAccessibilityMetadata : IAccessibilityMetadata
{
    public WeChatAccessibilityMetadata()
    {
        SupportedProcessNames = ["WeChat", "WeChatApp"];
        IdentityString = string.Join(",", SupportedProcessNames);
    }

    public string[] SupportedProcessNames { get; }
    public string IdentityString { get; }
}