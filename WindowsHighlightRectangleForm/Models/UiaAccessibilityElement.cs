using FlaUI.Core.AutomationElements;

namespace WindowsHighlightRectangleForm.Models;

public class UiaAccessibilityElement : UiAccessibilityElement, IUiAccessibilityElementReplayActions
{
    public static readonly Dictionary<string, UiaAccessibilityElement> UiaAccessibilityElements;

    static UiaAccessibilityElement()
    {
        var derivedTypes = GetDerivedTypes(typeof(UiaAccessibilityElement));
        var instances = InstantiateDerivedTypes(derivedTypes);
        UiaAccessibilityElements = instances.Where(c => c.Metadata != null)
            .ToDictionary(key => key.Metadata.IdentityString, value => value);
    }

    public IAccessibilityMetadata? Metadata { get; protected set; }

    public virtual void Click()
    {
        var accessibilityElement = GetAccessibilityElement();
        if (accessibilityElement != null)
        {
            accessibilityElement.Click();
        }
        else
        {
            CheckNativeElement(out var automationElement);
            automationElement.Click();
        }
    }

    private UiaAccessibilityElement? GetAccessibilityElement()
    {
        var processName = Accessibility.FileName;
        var findKey =
            UiaAccessibilityElements.Keys.FirstOrDefault(c =>
                c.Contains(processName, StringComparison.OrdinalIgnoreCase));
        return string.IsNullOrWhiteSpace(findKey) ? null : UiaAccessibilityElements[findKey];
    }

    private static List<UiaAccessibilityElement> InstantiateDerivedTypes(IEnumerable<Type> types)
    {
        var instances = new List<UiaAccessibilityElement>();

        foreach (var type in types)
            try
            {
                var instance = (UiaAccessibilityElement)Activator.CreateInstance(type);
                instances.Add(instance);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法实例化类型 {type.FullName}: {ex.Message}");
            }

        return instances;
    }

    private static IEnumerable<Type> GetDerivedTypes(Type baseType)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var derivedTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract);
            derivedTypes.AddRange(types);
        }

        return derivedTypes;
    }

    protected void CheckNativeElement(out AutomationElement automationElement)
    {
        if (NativeElement is not AutomationElement uiaElement)
            throw new InvalidOperationException(nameof(Click));
        automationElement = uiaElement;
    }
}