using System.Windows.Automation;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Infra.Windows.Win32;
using Tenon.Windows.UIA.Extensions;
using Process = System.Diagnostics.Process;

namespace UIRecorder;

public partial class MainForm : Form
{
    private readonly string _processName;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    private readonly WindowsHighlightBehavior _windowsHighlightBehavior;

    public MainForm()
    {
        InitializeComponent();
        _windowsHighlightBehavior = new WindowsHighlightBehavior();
        _windowsHighlight = new WindowsHighlightRectangle();
        _processName = string.Concat(Process.GetCurrentProcess().ProcessName, ".exe");
    }

    private async Task<Tuple<Rectangle, string>> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var point = new Point(location.X, location.Y);
            var hWnd = Window.GetTop(point);
            if (hWnd == IntPtr.Zero)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var processId = Window.GetProcessId(hWnd);
            var mainProcess = Process.GetProcessById((int)processId);
            var processName = mainProcess.ProcessName;
            var automationElement =
                AutomationElement.FromPoint(point);
            if (processName.Equals("WeChat") || processName.Equals("WeChatApp"))
                automationElement = FindChildDescendants(automationElement, point);
            if (automationElement == null)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var rect = automationElement.Current.BoundingRectangle;
            var controlType =
                (ControlType)automationElement.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty, false);
            return new Tuple<Rectangle, string>(rect, controlType?.ProgrammaticName?.Substring("ControlType.".Length));
        });
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

    public AutomationElement FindChildDescendants(AutomationElement parent, Point location)
    {
        var elementCollection = AutomationElementExtension.GetChildren(parent);
        foreach (var element in elementCollection)
            if (element.Current.BoundingRectangle.Contains(location))
            {
                var identifyElement = FindChildDescendants(element, location);
                if (identifyElement != null)
                    return identifyElement;
                var innerControlType =
                    (ControlType)element.GetCurrentPropertyValue(AutomationElement.ControlTypeProperty, false);
                if ((innerControlType != ControlType.Pane) & (innerControlType != ControlType.Window)
                                                           & (innerControlType != ControlType.Custom))

                    return element;
            }

        return null;
    }
}