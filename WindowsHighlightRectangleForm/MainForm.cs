using System.Text;
using FlaUI.Core.AutomationElements;
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
    private readonly string[] _ignoreProcessNames;
    private readonly IObjectMapper _mapper;
    private readonly ISerializer _serializer;
    private readonly UiaAccessibilityIdentity _uiaAccessibilityIdentity;
    private readonly UiAccessibility _uiAccessibility;
    private readonly UiAccessibilityIdentity _uiAccessibilityIdentity;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    private readonly WindowsHighlightBehavior _windowsHighlightBehavior;
    private bool _isLeftControl;
    private bool _isMouseDown;

    public MainForm(UiAccessibility uiAccessibility, UiAccessibilityIdentity uiAccessibilityIdentity,
        ISerializer serializer, IObjectMapper mapper, UiaAccessibilityIdentity uiaAccessibilityIdentity)
    {
        _uiAccessibility = uiAccessibility;
        _uiAccessibilityIdentity = uiAccessibilityIdentity;
        _serializer = serializer;
        _mapper = mapper;
        _uiaAccessibilityIdentity = uiaAccessibilityIdentity;
        InitializeComponent();
        _windowsHighlightBehavior = new WindowsHighlightBehavior();
        _windowsHighlight = new WindowsHighlightRectangle();
        _ignoreProcessNames = [Process.GetCurrentProcess().ProcessName];
    }

    private async Task<UiAccessibilityElement?> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var hwNd = Window.Get(location);
            if (hwNd == IntPtr.Zero) return null;
            var higherProcessName = Process.GetProcessById((int)Window.GetProcessId(hwNd)).ProcessName;
            if (_ignoreProcessNames.Contains(higherProcessName, StringComparer.OrdinalIgnoreCase))
                return null;
            var hoveredElement = _uiAccessibilityIdentity.FromPoint(location);
            return hoveredElement;
        });
    }


    private void AddLog(string message)
    {
        listBox1.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}"));
    }


    private void button1_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.MouseMoveEventHandler += MouseMoveEventHandler;
        _windowsHighlightBehavior.MouseDownEventHandler += MouseDownEventHandler;
        _windowsHighlightBehavior.MouseUpEventHandler += MouseUpEventHandler;
        _windowsHighlightBehavior.KeyDownEventHandler += KeyDownEventHandler;
        _windowsHighlightBehavior.KeyUpEventHandler += KeyUpEventHandler;
        _windowsHighlightBehavior.Start();
    }

    private void MouseUpEventHandler(object? sender, MouseEventArgs e)
    {
        _isMouseDown = false;
        AddLog($"isMouseDown:{_isMouseDown},isLeftControl:{_isLeftControl}");
    }

    private void MouseDownEventHandler(object? sender, MouseEventArgs e)
    {
        _isMouseDown = true;
        AddLog($"isMouseDown:{_isMouseDown},isLeftControl:{_isLeftControl}");
    }

    private void KeyUpEventHandler(object? sender, KeyEventArgs e)
    {
        _isLeftControl = false;
        AddLog($"isMouseDown:{_isMouseDown},isLeftControl:{_isLeftControl}");
    }

    private void KeyDownEventHandler(object? sender, KeyEventArgs e)
    {
        _isLeftControl = e.KeyCode == Keys.LControlKey;
        AddLog($"isMouseDown:{_isMouseDown},isLeftControl:{_isLeftControl}");
    }

    private void MouseMoveEventHandler(object? sender, MouseEventArgs e)
    {
        try
        {
            var identifyElement = ElementFromPointAsync(e.Location).ConfigureAwait(false).GetAwaiter().GetResult();
            //AddLog($"identifyElement:{e.Location},controlType:{identifyElement?.ControlType}");
            if (identifyElement == null || identifyElement.BoundingRectangle.IsEmpty)
            {
                _windowsHighlight.Hide();
                return;
            }

            _windowsHighlight.SetLocation(identifyElement.BoundingRectangle, identifyElement.ControlType.ToString());
        }
        catch (Exception ex)
        {
            AddLog($"identifyElement failed,error:{ex.Message}");
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        _windowsHighlightBehavior.MouseMoveEventHandler -= MouseMoveEventHandler;
        _windowsHighlightBehavior.MouseDownEventHandler -= MouseDownEventHandler;
        _windowsHighlightBehavior.MouseUpEventHandler -= MouseUpEventHandler;
        _windowsHighlightBehavior.KeyDownEventHandler -= KeyDownEventHandler;
        _windowsHighlightBehavior.KeyUpEventHandler -= KeyUpEventHandler;
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
        var mainWindow = _uiaAccessibilityIdentity.DesktopElement.FindFirstChild(cf => cf.ByName("¼ÆËãÆ÷"));
        if (mainWindow != null)
        {
            var button1 = mainWindow.FindFirstDescendant(cf => cf.ByName("Ò»"))?.AsButton();
            if (button1 != null)
            {
                button1?.Invoke();
                _uiAccessibility.Record(button1);
                var jsonString = _serializer.SerializeObject(_uiAccessibility);
                File.WriteAllText("locator.path", jsonString, Encoding.UTF8);
                var findElement = _uiAccessibility.FindElement(jsonString);
                AddLog(findElement != null ? "find Element" : "not find Element");
                if (findElement is UiaAccessibilityElement uiaElement)
                    uiaElement.Click();
            }
        }
    }
}