using System.Collections.ObjectModel;
using Mortise.Accessibility.Locator.Abstractions;
using Recorder.Models;
using Tenon.Serialization.Abstractions;

namespace Recorder.ViewModels;

public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Nodes = new ObservableCollection<Node>
        {
            new("Animals", new ObservableCollection<Node>
            {
                new("Mammals", new ObservableCollection<Node>
                {
                    new("Lion"), new("Cat"), new("Zebra")
                })
            }),
            new("Animals", new ObservableCollection<Node>
            {
                new("Mammals", new ObservableCollection<Node>
                {
                    new("Lion"), new("Cat"), new("Zebra")
                })
            })
        };
    }

    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Node> Nodes { get; }
}