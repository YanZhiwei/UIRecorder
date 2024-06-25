using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Mortise.Accessibility.Abstractions;
using Mortise.Accessibility.Locator.Abstractions;
using Recorder.Messenger;
using Recorder.Models;
using Tenon.Automation.Windows;
using Tenon.Infra.Windows.Win32;
using Tenon.Infra.Windows.Win32.Hooks;
using Tenon.Serialization.Abstractions;
using Process = System.Diagnostics.Process;

namespace Recorder.ViewModels;

public partial class MainWindowViewModel
    : ViewModelBase
{
    private static readonly ManualResetEvent Mre = new(true);
    private readonly Accessible _accessible;
    private readonly IAccessibleLocatorStorage _accessibleLocatorStorage;
    private readonly string[] _ignoreProcessNames;
    private readonly ConcurrentStack<MouseEventArgs> _mouseDownQueue = new();
    private readonly ConcurrentStack<MouseEventArgs> _mouseMoveQueue = new();
    private readonly ISerializer _serializer;
    private readonly WindowsHighlightRectangle _windowsHighlight;
    private bool _isLeftControl;

    [ObservableProperty] private ObservableCollection<Node>? _locatorNodes;

    private bool _shutdown;
    private Thread? _workerThread;

    public MainWindowViewModel()
    {
    }

    public MainWindowViewModel(Accessible accessible, ISerializer serializer,
        IAccessibleLocatorStorage accessibleLocatorStorage)
    {
        _serializer = serializer;
        _accessible = accessible ?? throw new ArgumentNullException(nameof(accessible));
        _accessibleLocatorStorage = accessibleLocatorStorage ??
                                    throw new ArgumentNullException(nameof(accessibleLocatorStorage));
        LocatorNodes = new ObservableCollection<Node>();
        _windowsHighlight = new WindowsHighlightRectangle();
        _ignoreProcessNames = [Process.GetCurrentProcess().ProcessName, "devenv"];
    }

    partial void OnLocatorNodesChanging(ObservableCollection<Node>? value)
    {
        value ??= new ObservableCollection<Node>();
        _accessibleLocatorStorage.Load();
        if (!(_accessibleLocatorStorage?.AccessibleDict.Any() ?? false)) return;
        foreach (var accessibleItem in _accessibleLocatorStorage.AccessibleDict)
        {
            var locator = new Node(accessibleItem.Key, []);
            foreach (var accessible in accessibleItem.Value)
                locator.SubNodes?.Add(new Node(accessible.UniqueId));
            value.Add(locator);
        }
    }

    private void Hook_ButtonUp(object? sender, MouseEventArgs e)
    {
        _isLeftControl = false;
        _mouseDownQueue.Clear();
    }

    private void Hook_KeyUp(object? sender, KeyEventArgs e)
    {
        _isLeftControl = false;
        _mouseDownQueue.Clear();
    }

    private void Hook_KeyDown(object? sender, KeyEventArgs e)
    {
        _isLeftControl = true;
    }

    private void Hook_MouseDown(object? sender, MouseEventArgs e)
    {
        _mouseDownQueue.Push(e);
    }

    private void Hook_MouseMove(object? sender, MouseEventArgs e)
    {
        _mouseMoveQueue.Push(e);
    }

    private void ThreadProcedure(object? obj)
    {
        var currentPosition = Cursor.Position;
        _mouseMoveQueue.Push(
            new MouseEventArgs(MouseButtons.None, 0, currentPosition.X, currentPosition.Y, 0));
        while (_shutdown)
            try
            {
                Mre.WaitOne();
                AccessibleComponent? element;
                if (_mouseDownQueue.TryPop(out var ee))
                {
                    _mouseMoveQueue.Clear();
                    _mouseDownQueue.Clear();
                    if (ee.Location.IsEmpty)
                    {
                        _windowsHighlight.Hide();
                        continue;
                    }

                    if (_isLeftControl)
                    {
                        element = ElementFromPointAsync(ee.Location).ConfigureAwait(false).GetAwaiter().GetResult();
                        if (element is { BoundingRectangle.IsEmpty: false })
                        {
                            _windowsHighlight.SetLocation(element.BoundingRectangle, element.ControlType.ToString());
                            _accessible.Record(element.NativeElement);
                            SyncAccessibleToNode(_accessible);
                        }
                        else
                        {
                            _windowsHighlight.Hide();
                        }
                    }
                }

                if (_mouseMoveQueue.TryPop(out var e))
                {
                    _mouseMoveQueue.Clear();
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

    private void SyncAccessibleToNode(Accessible? accessible)
    {
        if (accessible == null) return;
        if (_accessibleLocatorStorage.Add(accessible))
        {
            var existNode =
                LocatorNodes.FirstOrDefault(
                    c => c.Title.Equals(accessible.FileName, StringComparison.OrdinalIgnoreCase));
            if (existNode == null)
            {
                var newNode = new Node(accessible.FileName, []);
                newNode?.SubNodes?.Add(new Node(accessible.UniqueId));
                LocatorNodes.Add(newNode);
            }
            else
            {
                existNode?.SubNodes?.Add(new Node(accessible.UniqueId));
            }
        }
    }


    private async Task<AccessibleComponent?> ElementFromPointAsync(Point location)
    {
        return await Task.Factory.StartNew(() =>
        {
            var hwNd = Window.Get(location);
            if (hwNd == IntPtr.Zero) return null;
            var higherProcessName = Process.GetProcessById((int)Window.GetProcessId(hwNd)).ProcessName;
            if (_ignoreProcessNames.Contains(higherProcessName, StringComparer.OrdinalIgnoreCase))
                return null;
            var hoveredElement = _accessible.Identity.FromPoint(location);
            return hoveredElement;
        });
    }

    [RelayCommand]
    private void Record()
    {
        _shutdown = true;
        MouseHook.Install();
        KeyboardHook.Install();
        MouseHook.MouseMove += Hook_MouseMove;
        MouseHook.LeftButtonDown += Hook_MouseDown;
        MouseHook.RightButtonDown += Hook_MouseDown;
        MouseHook.LeftButtonUp += Hook_ButtonUp;
        MouseHook.RightButtonUp += Hook_ButtonUp;
        KeyboardHook.KeyDown += Hook_KeyDown;
        KeyboardHook.KeyUp += Hook_KeyUp;
        if (_workerThread == null)
        {
            _workerThread = new Thread(ThreadProcedure)
            {
                Priority = ThreadPriority.AboveNormal,
                IsBackground = true
            };
            _workerThread.SetApartmentState(ApartmentState.STA);
        }

        _workerThread.Start();
        Mre.Set();
    }

    [RelayCommand]
    private void Save()
    {
        _accessibleLocatorStorage.Save();
        WeakReferenceMessenger.Default.Send(new CloseWindowMessage
        {
            Sender = new WeakReference(this)
        });
    }
}