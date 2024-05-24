using System.Diagnostics;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace UIRecorder.Models;

internal class UiaAccessibility : UiAccessibility
{
    private readonly UIA3Automation _automation = new();
    private readonly AutomationElement _rootElement;
    private readonly AutomationElement _targetElement;
    private readonly ITreeWalker _treeWalker;

    public UiaAccessibility(AutomationElement automationElement)
    {
        _targetElement = automationElement ?? throw new ArgumentNullException(nameof(automationElement));
        Process = Process.GetProcessById(automationElement.Properties.ProcessId);
        Name = nameof(UiaAccessibility);
        Type = "UIA";
        Version = new Version(3, 0, 0);
        _treeWalker = _automation.TreeWalkerFactory.GetControlViewWalker();
        _rootElement = _automation.GetDesktop();
    }

    public override Stack<UiAccessibilityElement> GetElementStack()
    {
        var uiaElementPaths = new Stack<AutomationElement>();
        var targetElement = _targetElement;
        uiaElementPaths.Push(targetElement);
        while (targetElement.Parent != null && targetElement.Parent != _rootElement)
        {
            targetElement = _treeWalker.GetParent(targetElement);
            uiaElementPaths.Push(targetElement);
        }

        AutomationElement parentElement = null;
        while (uiaElementPaths.Count > 0)
        {
            parentElement = uiaElementPaths.Pop();
            var controlType = parentElement.ControlType;
            UiAccessibilityElement element = new UiAccessibilityElement();
            switch (controlType)
            {
                case ControlType.Edit:
                    element.Name = parentElement.Name;
                    break;
            }

        }
        return null;
    }
}