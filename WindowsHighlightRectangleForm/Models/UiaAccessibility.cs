using System.Diagnostics;
using System.Runtime.InteropServices;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.UIA3.Identifiers;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibility : UiAccessibility
{
    private const string FRAME_WINDOW = "ApplicationFrameWindow";
    protected readonly UiaAccessibilityIdentity Identity;

    public UiaAccessibility(UiaAccessibilityIdentity uiaAccessibilityIdentity)
    {
        Identity = uiaAccessibilityIdentity ??
                   throw new ArgumentNullException(nameof(uiaAccessibilityIdentity));
        Technology = UiAccessibilityTechnology.Uia;
        Platform = PlatformID.Win32NT;
        Version = new Version(3, 0, 0);
    }

    protected IntPtr WindowHandle { get; set; }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
        string lpszWindow);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    protected virtual AutomationElement GetWindowElement()
    {
        var process = Process.GetProcessById(ProcessId);
        var mainWindowHandle = process?.MainWindowHandle ?? WindowHandle;
        if (mainWindowHandle == IntPtr.Zero)
        {
            var windows = FindAppWindows(process);
            mainWindowHandle = windows.FirstOrDefault();
        }

        return mainWindowHandle == IntPtr.Zero
            ? Identity.DesktopElement
            : Identity.Automation.FromHandle(mainWindowHandle);
    }

    private List<IntPtr> FindAppWindows(Process proc)
    {
        var appWindows = new List<IntPtr>();
        for (var appWindow = FindWindowEx(IntPtr.Zero, IntPtr.Zero, FRAME_WINDOW, null);
             appWindow != IntPtr.Zero;
             appWindow = FindWindowEx(IntPtr.Zero, appWindow, FRAME_WINDOW, null))
        {
            var coreWindow = FindWindowEx(appWindow, IntPtr.Zero, "Windows.UI.Core.CoreWindow", null);
            if (coreWindow != IntPtr.Zero)
            {
                GetWindowThreadProcessId(coreWindow, out var corePid);
                if (corePid == proc.Id) 
                    appWindows.Add(appWindow);
            }
        }

        return appWindows;
    }

    public override void Record(object element)
    {
        if (element is not AutomationElement automationElement) throw new NotSupportedException(nameof(element));
        ProcessId = automationElement.Properties.ProcessId;
        var uiaElementPaths = new DistinctStack<UiAccessibilityElement>();
        var currentElement = automationElement;
        FileName = Process.GetProcessById(currentElement.Properties.ProcessId).ProcessName;
        uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        while (currentElement.Parent != null)
        {
            if (currentElement.Parent.Equals(Identity.DesktopElement))
                break;
            currentElement = Identity.TreeWalker.GetParent(currentElement);
            uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement)!);
        }

        RecordElements = uiaElementPaths;
    }

    public override void Replay()
    {
        AutomationElement? foundElement = null;
        var parentElement = GetWindowElement();
        while (RecordElements.TryPop(out var item))
        {
            if (item.Element is not AutomationElement automationElement) break;
            if (parentElement == null) continue;
            var condition = CreateConditionFromElement(automationElement);
            foundElement = parentElement.FindFirstDescendant(condition);
            parentElement = foundElement ?? Identity.TreeWalker.GetNextSibling(parentElement);
        }

        if (foundElement != null)
            Console.WriteLine("Element found with the generated condition.");
        else
            Console.WriteLine("Element not found with the generated condition.");
    }

    private static ConditionBase CreateConditionFromElement(AutomationElement element)
    {
        var conditions = new List<ConditionBase>();
        if (element.Properties.AutomationId.IsSupported)
        {
            var automationId = element.Properties.AutomationId.ValueOrDefault;
            conditions.Add(new PropertyCondition(AutomationObjectIds.AutomationIdProperty, automationId));
        }

        if (element.Properties.Name.IsSupported)
        {
            var name = element.Properties.Name.ValueOrDefault;
            conditions.Add(new PropertyCondition(AutomationObjectIds.NameProperty, name));
        }

        if (element.Properties.ControlType.IsSupported)
        {
            var controlType = element.Properties.ControlType.ValueOrDefault;
            conditions.Add(new PropertyCondition(AutomationObjectIds.ControlTypeProperty, controlType));
        }

        // 将所有条件合并为一个 AndCondition
        return new AndCondition(conditions.ToArray());
    }
}