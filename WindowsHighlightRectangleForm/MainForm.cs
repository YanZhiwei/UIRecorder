using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Mapper.Abstractions;
using Tenon.Serialization.Abstractions;
using WindowsHighlightRectangleForm.Models;
using Process = System.Diagnostics.Process;

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

    private async Task<Tuple<Rectangle, string>> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var hoveredElement = _uiAccessibilityIdentity.FromPoint(location);
            if (hoveredElement == null)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var rect = hoveredElement.BoundingRectangle;
            var controlType =
                hoveredElement.ControlType;
            return new Tuple<Rectangle, string>(rect, controlType.ToString());
        });
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
        var mainWindow = _uiaAccessibilityIdentity.DesktopElement.FindFirstChild(cf => cf.ByName("¼ÆËãÆ÷"));
        if (mainWindow != null)
        {
            var button1 = mainWindow.FindFirstDescendant(cf => cf.ByName("Ò»"))?.AsButton();
            if (button1 != null)
            {
                button1?.Invoke();
                _uiAccessibility.Record(button1);
                var jsonString = _serializer.SerializeObject(_uiAccessibility);
                Debug.WriteLine(jsonString);
                _uiAccessibility.Replay();
            }
        }
    }
}