using System.Collections.ObjectModel;
using System.Xml.Linq;
using Recorder.Models;

namespace Recorder.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Node> Nodes { get; }

    public MainViewModel()
    {
        Nodes = new ObservableCollection<Node>
        {
            new Node("Animals", new ObservableCollection<Node>
            {
                new Node("Mammals", new ObservableCollection<Node>
                {
                    new Node("Lion"), new Node("Cat"), new Node("Zebra")
                })
            })
        };
    }
}
