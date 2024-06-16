using System.Collections.Concurrent;
using Mortise.Accessibility.Abstractions;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Form.Common;
using Tenon.Infra.Windows.Win32.Hooks;
using Tenon.Serialization.Abstractions;
using Process = System.Diagnostics.Process;
using Window = Tenon.Infra.Windows.Win32.Window;

namespace WindowsHighlightSample;

public partial class MainForm : Form
{
    private static readonly object SyncRoot = new();
    protected static readonly ManualResetEvent Mre = new(true);
    private readonly Accessible _accessible;
    private readonly string[] _ignoreProcessNames;
    private readonly ISerializer _serializer;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    protected readonly ConcurrentStack<MouseEventArgs> MouseDownQueue = new();
    protected readonly ConcurrentStack<MouseEventArgs> MouseMoveQueue = new();
    private bool _isLeftControl;
    private bool _shutdown;
    protected Thread? WorkerThread;

    public MainForm(Accessible accessible, ISerializer serializer)
    {
        _accessible = accessible ?? throw new ArgumentNullException(nameof(accessible));
        _serializer = serializer;
        InitializeComponent();
        _windowsHighlight = new WindowsHighlightRectangle();
        _ignoreProcessNames = [Process.GetCurrentProcess().ProcessName];
    }

    private async Task<AccessibleComponent?> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var hwNd = Window.Get(location);
            if (hwNd == IntPtr.Zero) return null;
            var higherProcessName = Process.GetProcessById((int) Window.GetProcessId(hwNd)).ProcessName;
            if (_ignoreProcessNames.Contains(higherProcessName, StringComparer.OrdinalIgnoreCase))
                return null;
            var hoveredElement = _accessible.Identity.FromPoint(location);
            return hoveredElement;
        });
    }


    private void AddLog(string message)
    {
        listBox1.UIBeginThread(ls => ls.AddItemSelected($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}"));
    }


    private void button1_Click(object sender, EventArgs e)
    {
        lock (SyncRoot)
        {
            _shutdown = true;
            MouseHook.Install();
            KeyboardHook.Install();
            MouseHook.MouseMove += Hook_MouseMove;
            MouseHook.LeftButtonDown += Hook_MouseDown;
            MouseHook.RightButtonDown += Hook_MouseDown;
            MouseHook.LeftButtonUp += Hook_UpDown;
            MouseHook.RightButtonUp += Hook_UpDown;
            KeyboardHook.KeyDown += Hook_KeyDown;
            KeyboardHook.KeyUp += Hook_KeyUp;
            if (WorkerThread == null)
            {
                WorkerThread = new Thread(ThreadProcedure)
                {
                    Priority = ThreadPriority.AboveNormal,
                    IsBackground = true
                };
                WorkerThread.SetApartmentState(ApartmentState.STA);
            }

            WorkerThread.Start();
            Mre.Set();
        }
    }

    private void Hook_KeyUp(object? sender, KeyEventArgs e)
    {
        _isLeftControl = false;
    }

    private void Hook_KeyDown(object? sender, KeyEventArgs e)
    {
        _isLeftControl = true;
    }

    private void Hook_UpDown(object? sender, MouseEventArgs e)
    {
        MouseDownQueue.Clear();
    }

    private void Hook_MouseDown(object? sender, MouseEventArgs e)
    {
        MouseDownQueue.Push(e);
    }

    private void Hook_MouseMove(object? sender, MouseEventArgs e)
    {
        MouseMoveQueue.Push(e);
    }

    private void ThreadProcedure(object? obj)
    {
        var currentPosition = Cursor.Position;
        MouseMoveQueue.Push(
            new MouseEventArgs(MouseButtons.None, 0, currentPosition.X, currentPosition.Y, 0));
        while (_shutdown)
            try
            {
                Mre.WaitOne(); //�ȴ��ź�
                AccessibleComponent? element;
                if (MouseDownQueue.TryPop(out var ee))
                {
                    MouseMoveQueue.Clear();
                    MouseDownQueue.Clear();
                    if (ee.Location.IsEmpty)
                    {
                        _windowsHighlight.Hide();
                        continue;
                    }

                    if (_isLeftControl)
                    {
                        element = ElementFromPointAsync(ee.Location).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (element is {BoundingRectangle.IsEmpty: false})
                        {
                            _windowsHighlight.SetLocation(element.BoundingRectangle, element.ControlType.ToString());
                            _accessible.Record(element.NativeElement);
                            var jsonString = _serializer.SerializeObject(_accessible);
                            AddLog($"capture element:{jsonString}");
                        }
                        else
                        {
                            _windowsHighlight.Hide();
                        }
                    }
                }

                if (MouseMoveQueue.TryPop(out var e))
                {
                    MouseMoveQueue.Clear();
                    if (e.Location.IsEmpty) continue;
                    element = ElementFromPointAsync(e.Location).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (element == null || element.BoundingRectangle.IsEmpty)
                    {
                        _windowsHighlight.Hide();
                        continue;
                    }

                    _windowsHighlight.SetLocation(element.BoundingRectangle, element.ControlType.ToString());
                }
            }
            catch
            {
                // ignored
            }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        lock (SyncRoot)
        {
            _shutdown = false;
            _windowsHighlight.Hide();
            WorkerThread?.Interrupt();
            WorkerThread = null;
            MouseMoveQueue.Clear();
            MouseDownQueue.Clear();
            MouseHook.MouseMove -= Hook_MouseMove;
            MouseHook.LeftButtonDown -= Hook_MouseDown;
            MouseHook.RightButtonDown -= Hook_MouseDown;
            MouseHook.LeftButtonUp -= Hook_UpDown;
            MouseHook.RightButtonUp -= Hook_UpDown;
            KeyboardHook.KeyDown -= Hook_KeyDown;
            KeyboardHook.KeyUp -= Hook_KeyUp;
            MouseHook.Uninstall();
            KeyboardHook.Uninstall();
        }
    }

    private void button3_Click(object sender, EventArgs e)
    {
        if (WorkerThread != null)
            Mre.Reset();
    }

    private void button4_Click(object sender, EventArgs e)
    {
        if (WorkerThread != null)
            Mre.Set();
    }

    private void button5_Click(object sender, EventArgs e)
    {
        //var mainWindow = _accessible.Identity.DesktopElement.FindFirstChild(cf => cf.ByName("������"));
        //if (mainWindow != null)
        //{
        //    var buttonTest = mainWindow.FindFirstDescendant(cf => cf.ByName("һ"))?.AsButton();
        //    if (buttonTest != null)
        //    {
        //        buttonTest?.Invoke();
        //        _uiaAccessibility.Record(buttonTest);
        //        var jsonString = _serializer.SerializeObject(_uiaAccessibility);
        //        File.WriteAllText("locator.path", jsonString, Encoding.UTF8);
        //        var findElement = _uiaAccessibility.FindComponent(jsonString);
        //        AddLog(findElement != null ? "find Element" : "not find Element");
        //        //if (findElement is AccessibleComponent uiaElement)
        //        //    uiaElement.Click();
        //    }
        //}
    }
}