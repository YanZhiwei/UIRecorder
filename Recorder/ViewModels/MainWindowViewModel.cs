using System.Collections.ObjectModel;
using Mortise.Accessibility.Locator.Abstractions;
using Recorder.Models;
using Tenon.Serialization.Abstractions;

namespace Recorder.ViewModels;

public class MainWindowViewModel(ISerializer serializer, IAccessibleLocatorStorage accessibleLocatorStorage)
    : ViewModelBase
{
    private readonly IAccessibleLocatorStorage _accessibleLocatorStorage = accessibleLocatorStorage;
    private readonly ISerializer _serializer = serializer;

    public ObservableCollection<Node> Nodes { get; } =
    [
        new Node("Animals", new ObservableCollection<Node>
        {
            new("Mammals", new ObservableCollection<Node>
            {
                new("Lion"), new("Cat"), new("Zebra")
            })
        }),

        new Node("Animals", new ObservableCollection<Node>
        {
            new("Mammals", new ObservableCollection<Node>
            {
                new("Lion"), new("Cat"), new("Zebra")
            })
        })
    ];
}