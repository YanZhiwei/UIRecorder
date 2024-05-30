using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;
using WindowsHighlightRectangleForm.Models;
using Process = System.Diagnostics.Process;
using Window = Tenon.Infra.Windows.Win32.Window;

namespace WindowsHighlightRectangleForm;

public partial class MainForm : Form
{
    private readonly Dictionary<string, UiAccessibilityIdentity> _accessibilityIdentities;
    private readonly UIA3Automation _automation = new();
    private readonly UiAccessibilityIdentity _defaultAccessibilityIdentity;
    private readonly string[] _ignoreProcessNames;
    private readonly IObjectMapper _mapper;
    private readonly AutomationElement _rootElement;
    private readonly ISerializer _serializer;
    private readonly ITreeWalker _treeWalker;
    private readonly UiAccessibilityIdentity _weChatAccessibilityIdentity;
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
        _accessibilityIdentities = CreateUiAccessibilityIdentityInstances<UiAccessibilityIdentity>()
            .ToDictionary(key => key.ProcessName, value => value);
        _defaultAccessibilityIdentity = _accessibilityIdentities["*"];
        _weChatAccessibilityIdentity = _accessibilityIdentities["WeChat"];
    }

    //private async Task<Tuple<Rectangle, string>> ElementFromPointAsync(Point location)
    //{
    //    //var hoveredPoint = new Point(location.X, location.Y);
    //    //var windowTopHWnd = Window.GetTop(hoveredPoint);
    //    //if (windowTopHWnd == IntPtr.Zero)
    //    //    return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
    //    //var processId = Window.GetProcessId(windowTopHWnd);
    //    //if (processId == IntPtr.Zero)
    //    //    return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
    //    //var hoveredProcess = Process.GetProcessById((int)processId);
    //    return await Task.Factory.StartNew(() =>
    //    {
    //        var point = new Point(location.X, location.Y);
    //        var element =
    //            _automation.FromPoint(point);
    //        if (element == null)
    //            return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
    //        AutomationElement? hoveredElement = null;
    //        var hoveredProcess = Process.GetProcessById(element.Properties.ProcessId);
    //        //if (_accessibilityIdentities.TryGetValue(hoveredProcess.ProcessName, out var uiAccessibilityIdentity))
    //        //    hoveredElement = uiAccessibilityIdentity.FromPoint(location.X, location.Y);
    //        //else
    //            hoveredElement = _defaultAccessibilityIdentity.FromPoint(location.X, location.Y);
    //        if (hoveredElement == null)
    //            return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
    //        return new Tuple<Rectangle, string>(hoveredElement.BoundingRectangle,
    //            hoveredElement.ControlType.ToString());
    //    });
    //}

    private async Task<Tuple<Rectangle, string>> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var point = new Point(location.X, location.Y);
            var hoveredElement =
                _automation.FromPoint(point);
            if (hoveredElement == null)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            _treeWalker.GetParent(hoveredElement);
            var processName = Process.GetProcessById(hoveredElement.Properties.ProcessId).ProcessName;
            //var hoveredPoint = new Point(location.X, location.Y);
            //var windowTopHWnd = Window.GetTop(hoveredPoint);
            //if (windowTopHWnd == IntPtr.Zero)
            //    return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            //var processId = Window.GetProcessId(windowTopHWnd);
            //if (processId == IntPtr.Zero)
            //    return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            //var processName = Process.GetProcessById((int)processId).ProcessName;
            //if (_ignoreProcessNames.Contains(processName, StringComparer.OrdinalIgnoreCase))
            //    return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            //var hoveredElement = _defaultAccessibilityIdentity.FromPoint(location.X, location.Y);
            //if (hoveredElement != null)
            //    return new Tuple<Rectangle, string>(hoveredElement.BoundingRectangle,
            //        hoveredElement.ControlType.ToString());
            if (processName.Equals("WeChat") || processName.Equals("WeChatApp") || processName.Equals("DingTalk"))
                // hoveredElement = hoveredElement.FindChildDescendants(point) ?? hoveredElement;
                hoveredElement = _weChatAccessibilityIdentity.FromPoint(location.X, location.Y) ?? hoveredElement;
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
            var identifyElement = ElementFromPointAsync(e.Location).ConfigureAwait(false).GetAwaiter()
                .GetResult();
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
                uiaAccessibility.GetElementStack();
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