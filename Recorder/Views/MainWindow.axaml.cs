using Avalonia.Controls;
using Avalonia.Input;

namespace Recorder.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PointerPressed += MainWindow_PointerPressed;
    }

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Mouse) BeginMoveDrag(e);
    }
}