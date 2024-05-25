using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using UIRecorder.Models;
using Process = System.Diagnostics.Process;

namespace UIRecorder;

public partial class MainForm : Form
{
    private readonly UIA3Automation _automation = new();
    private readonly string _processName;
    private readonly AutomationElement _rootElement;
    private readonly ITreeWalker _treeWalker;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    private readonly WindowsHighlightBehavior _windowsHighlightBehavior;

    public MainForm()
    {
        InitializeComponent();
        _windowsHighlightBehavior = new WindowsHighlightBehavior();
        _windowsHighlight = new WindowsHighlightRectangle();
        _processName = string.Concat(Process.GetCurrentProcess().ProcessName, ".exe");
        _treeWalker = _automation.TreeWalkerFactory.GetControlViewWalker();
        _rootElement = _automation.GetDesktop();
    }

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
            //ElementToSelectChanged(hoveredElement);
            var processName = Process.GetProcessById(hoveredElement.Properties.ProcessId).ProcessName;
            if (processName.Equals("WeChat") || processName.Equals("WeChatApp") || processName.Equals("DingTalk"))
                hoveredElement = hoveredElement.FindChildDescendants(point) ?? hoveredElement;
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
        _windowsHighlightBehavior.Start();
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
                UiaAccessibility uiaAccessibility = new UiaAccessibility(button1);
                uiaAccessibility.GetElementStack();
            }
        }
    }
}