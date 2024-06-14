﻿using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3.Identifiers;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;
using Tenon.Windows.Extensions;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibility : UiAccessibility
{
    public readonly UiaAccessibilityIdentity Identity;
    protected readonly IObjectMapper Mapper;
    protected readonly ISerializer Serializer;

    public UiaAccessibility(IObjectMapper mapper,
        ISerializer serializer)
    {
        Identity = new UiaAccessibilityIdentity(mapper);
        Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        Serializer = serializer;
        Technology = UiAccessibilityTechnology.Uia;
        Platform = PlatformID.Win32NT;
        Version = new Version(3, 0, 0);
    }

    public UiaAccessibility()
    {
    }

    protected IntPtr WindowHandle { get; set; }

    protected virtual AutomationElement GetWindowElement(string processName)
    {
        var process = Process.GetProcessesByName(processName).FirstOrDefault();
        var mainWindowHandle = process?.MainWindowHandle ?? WindowHandle;
        if (mainWindowHandle == IntPtr.Zero)
            mainWindowHandle = process.FindFirstCoreWindows();

        return mainWindowHandle == IntPtr.Zero
            ? Identity.DesktopElement
            : Identity.Automation.FromHandle(mainWindowHandle);
    }

    public override void Record(object element)
    {
        if (element is not AutomationElement automationElement) throw new NotSupportedException(nameof(element));
        var uiaElementPaths = new DistinctStack<UiAccessibilityElement>();
        var currentElement = automationElement;
        FileName = Process.GetProcessById(currentElement.Properties.ProcessId).ProcessName;
        uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement, this)!);
        while (currentElement.Parent != null)
        {
            if (currentElement.Parent.Equals(Identity.DesktopElement))
                break;
            currentElement = Identity.TreeWalker.GetParent(currentElement);
            uiaElementPaths.Push(Identity.DtoAccessibilityElement(currentElement, this)!);
        }

        RecordElements = uiaElementPaths;
    }

    public override UiAccessibilityElement? FindElement(string locatorPath)
    {
        if (string.IsNullOrEmpty(locatorPath))
            throw new ArgumentNullException(nameof(locatorPath));

        var uiaAccessibility = Serializer.DeserializeObject<UiaAccessibility>(locatorPath);
        AutomationElement? foundElement = null;
        var parentElement = GetWindowElement(uiaAccessibility.FileName);
        var recordElements = new Stack<UiAccessibilityElement>(uiaAccessibility.RecordElements);
        while (recordElements.TryPop(out var item))
        {
            var condition = CreateCondition(item);
            foundElement = parentElement.FindFirstDescendant(condition);
            if (foundElement == null)
            {
                parentElement = Identity.TreeWalker.GetParent(parentElement);
                foundElement = parentElement.FindFirstChild(condition);
            }

            if (foundElement == null) break;
            parentElement = foundElement;
        }

        return foundElement != null ? Identity.DtoAccessibilityElement(foundElement, this) : null;
    }

    protected virtual ConditionBase CreateCondition(UiAccessibilityElement element)
    {
        var conditions = new ConditionBase[]
        {
            !string.IsNullOrEmpty(element.Id)
                ? new PropertyCondition(AutomationObjectIds.AutomationIdProperty, element.Id)
                : new PropertyCondition(AutomationObjectIds.AutomationIdProperty, ""),
            !string.IsNullOrEmpty(element.Name)
                ? new PropertyCondition(AutomationObjectIds.NameProperty, element.Name)
                : new PropertyCondition(AutomationObjectIds.NameProperty, ""),
            new PropertyCondition(AutomationObjectIds.IsDialogProperty, element.IsDialog),
            new PropertyCondition(AutomationObjectIds.ControlTypeProperty,
                Mapper.Map<ControlType>(element.ControlType))
        };
        return new AndCondition(conditions);
    }
}