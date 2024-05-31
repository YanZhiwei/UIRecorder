using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;
using WindowsHighlightRectangleForm.Models;
using Process = System.Diagnostics.Process;

namespace WindowsHighlightRectangleForm;

public partial class MainForm : Form
{
    private readonly UIA3Automation _automation = new();
    private readonly UiAccessibilityIdentity _defaultAccessibilityIdentity;
    private readonly string[] _ignoreProcessNames;
    private readonly IObjectMapper _mapper;
    private readonly AutomationElement _rootElement;
    private readonly ISerializer _serializer;
    private readonly ITreeWalker _treeWalker;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    private readonly WindowsHighlightBehavior _windowsHighlightBehavior;

    public MainForm(ISerializer serializer, IObjectMapper mapper)
    {
        _serializer = serializer;
        _mapper = mapper;
        InitializeComponent();
        _windowsHighlightBehavior = new WindowsHighlightBehavior();
        _windowsHighlight = new WindowsHighlightRectangle();
        _ignoreProcessNames = [Process.GetCurrentProcess().ProcessName];
        _treeWalker = _automation.TreeWalkerFactory.GetControlViewWalker();
        _rootElement = _automation.GetDesktop();
        _defaultAccessibilityIdentity = new UiaAccessibilityIdentity();
    }

    private async Task<Tuple<Rectangle, string>> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var hoveredElement = _defaultAccessibilityIdentity.FromPoint(location);
            if (hoveredElement == null)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var rect = hoveredElement.BoundingRectangle;
            var controlType =
                hoveredElement.ControlType;
            return new Tuple<Rectangle, string>(rect, controlType.ToString());
        });
    }

    private void ElementToSelectChanged(AutomationElement obj)
    {
        var pathToRoot = new Stack<AutomationElement>();
        while (obj != null)
        {
            if (pathToRoot.Contains(obj) || obj.Equals(_rootElement)) break;

            pathToRoot.Push(obj);
            try
            {
                obj = _treeWalker.GetParent(obj);
            }
            catch (Exception ex)
            {
                // ignored
            }
        }
    }

    private void AddLog(string message)
    {
        listBox1.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}"));
    }


    private void button1_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.MouseMoveEventHandler += IdentifyFromPoint;
        _windowsHighlightBehavior.KeyDownEventHandler += KeyDownEventHandler;
        _windowsHighlightBehavior.Start();
    }

    private void KeyDownEventHandler(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.LControlKey) AddLog("get locator");
    }

    private void IdentifyFromPoint(object? sender, MouseEventArgs e)
    {
        try
        {
            var identifyElement = ElementFromPointAsync(e.Location).ConfigureAwait(false).GetAwaiter().GetResult();
            AddLog($"identifyElement:{e.Location},className:{identifyElement.Item2}");
            if (identifyElement.Item1.IsEmpty)
            {
                _windowsHighlight.Hide();
                return;
            }

            _windowsHighlight.SetLocation(identifyElement.Item1, identifyElement.Item2);
        }
        catch (Exception ex)
        {
            AddLog($"identifyElement failed,error:{ex.Message}");
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.MouseMoveEventHandler -= IdentifyFromPoint;
        _windowsHighlightBehavior.KeyDownEventHandler -= KeyDownEventHandler;
        _windowsHighlightBehavior.Stop();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.Suspend();
    }

    private void button4_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.Resume();
    }

    private void button5_Click(object sender, EventArgs e)
    {
        var mainWindow = _rootElement.FindFirstChild(cf => cf.ByName("¼ÆËãÆ÷"));
        if (mainWindow != null)
        {
            var button1 = mainWindow.FindFirstDescendant(cf => cf.ByName("Ò»"))?.AsButton();
            if (button1 != null)
            {
                button1?.Invoke();
                var uiaAccessibility = new UiaAccessibility(button1, _serializer, _mapper);
               // uiaAccessibility.Snapshotting();
            }
        }
    }

    private void button6_Click(object sender, EventArgs e)
    {
        var derivedInstances = CreateUiAccessibilityIdentityInstances<UiAccessibilityIdentity>();

        foreach (var instance in derivedInstances)
            AddLog(instance.ProcessName);
    }

    public static IEnumerable<T> CreateUiAccessibilityIdentityInstances<T>() where T : class
    {
        var derivedTypes = GetUiAccessibilityIdentityTypes(typeof(T));

        var instances = new List<T>();

        foreach (var type in derivedTypes)
            if (Activator.CreateInstance(type) is T instance)
                instances.Add(instance);

        return instances;
    }

    public static IEnumerable<Type> GetUiAccessibilityIdentityTypes(Type baseType)
    {
        var derivedTypes = new List<Type>();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var assembly in assemblies)
            derivedTypes.AddRange(assembly.GetTypes().Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract));

        return derivedTypes;
    }
}