using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;

namespace UIRecorder.Models;

internal class UiaAccessibility : UiAccessibility
{
    private readonly UIA3Automation _automation = new();
    private readonly AutomationElement _rootElement;
    private readonly ISerializer _serializer;
    private readonly AutomationElement _targetElement;
    private readonly ITreeWalker _treeWalker;
    private readonly IObjectMapper _mapper;
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
                ActualWidth = automationElement.ActualHeight
            };
            uiAccessibilityElement.ActualWidth = automationElement.ActualWidth;
            uiAccessibilityElement.BoundingRectangle = automationElement.BoundingRectangle;
            uiAccessibilityElement.Id = automationElement.Properties.AutomationId.ValueOrDefault;
            uiAccessibilityElement.IsEnabled = automationElement.Properties.IsEnabled.ValueOrDefault;
            uiAccessibilityElement.IsOffscreen = automationElement.Properties.IsOffscreen.ValueOrDefault;
            uiAccessibilityElement.IsDialog = automationElement.Properties.IsDialog.ValueOrDefault;
            return uiAccessibilityElement;
        }

        return null;
    }
}