using System.Windows.Automation;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Infra.Windows.Win32;
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

    private async Task<Tuple<Rectangle, string>> GetUiObjectFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var automationElement =
                AutomationElement.FromPoint(new Point(location.X, location.Y));
            if (automationElement == null)
                return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var hWnd = Window.Get(location);
            if (hWnd == IntPtr.Zero) return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var processId = Window.GetProcessId(hWnd);
            if (processId == IntPtr.Zero) return new Tuple<Rectangle, string>(Rectangle.Empty, string.Empty);
            var process = Process.GetProcessById((int)processId);
            var rect = automationElement.Current.BoundingRectangle;
            var className = automationElement.GetCurrentPropertyValue(AutomationElement.ClassNameProperty, false)
                ?.ToString();
            return new Tuple<Rectangle, string>(rect, className);
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
            AddLog($"IdentifyFromPoint:{e.Location}");
            var uiObjectInfo = GetUiObjectFromPointAsync(e.Location).ConfigureAwait(false).GetAwaiter()
                .GetResult();
            if (uiObjectInfo.Item1.IsEmpty)
            {
                _windowsHighlight.Hide();
                return;
            }

            _windowsHighlight.SetLocation(uiObjectInfo.Item1);
        }
        catch (Exception ex)
        {
            AddLog($"IdentifyFromPoint failed,error:{ex.Message}");
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
}