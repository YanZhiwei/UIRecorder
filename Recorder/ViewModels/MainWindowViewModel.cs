using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using Mortise.Accessibility.Locator.Abstractions;
using Mortise.UiaAccessibility;
using Recorder.Models;
using Tenon.Serialization.Abstractions;

namespace Recorder.ViewModels;

public class MainWindowViewModel
    : ViewModelBase
{
    private readonly IAccessibleLocatorStorage _accessibleLocatorStorage;
    private readonly string _appData;
    private readonly ISerializer _serializer;

    public MainWindowViewModel()
    {

    }
    public MainWindowViewModel(ISerializer serializer, IAccessibleLocatorStorage accessibleLocatorStorage)
    {
        _serializer = serializer;
        _accessibleLocatorStorage = accessibleLocatorStorage;
        _appData = Path.Combine(AppContext.BaseDirectory, "locators");
        Nodes = Initializelocators();
    }

    public ObservableCollection<Node> Nodes { get; } 

    private ObservableCollection<Node> Initializelocators()
    {
        var nodes = new ObservableCollection<Node>();
        if (!Directory.Exists(_appData)) return nodes;
        var locatorFiles = Directory.EnumerateFiles(_appData, "*.locator").ToArray();
        foreach (var locatorFile in locatorFiles)
        {
            var locatorName = Path.GetFileNameWithoutExtension(locatorFile);
            var locatorJsonString = File.ReadAllText(locatorFile, Encoding.UTF8);
            var accessibles = _serializer.DeserializeObject<UiaAccessible[]>(locatorJsonString);
            var locator = new Node(locatorName, new ObservableCollection<Node>());
            foreach (var accessible in accessibles) locator.SubNodes.Add(new Node(accessible.UniqueId));
            nodes.Add(locator);
        }

        return nodes;
    }
}