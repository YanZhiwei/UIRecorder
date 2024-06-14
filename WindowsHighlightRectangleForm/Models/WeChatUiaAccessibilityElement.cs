namespace WindowsHighlightRectangleForm.Models;

public sealed class WeChatUiaAccessibilityElement : UiaAccessibilityElement
{
    public WeChatUiaAccessibilityElement()
    {
        Metadata = new WeChatAccessibilityMetadata();
    }
}