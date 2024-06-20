using System.Collections.ObjectModel;

namespace Recorder.Models;

public class Node
{
    public Node(string title)
    {
        Title = title;
    }

    public Node(string title, ObservableCollection<Node> subNodes)
    {
        Title = title;
        SubNodes = subNodes;
    }

    public ObservableCollection<Node>? SubNodes { get; }
    public string Title { get; }
}