﻿using System.Text.Json.Serialization;

namespace WindowsHighlightRectangleForm.Models;

public class UiAccessibilityElement
{
    public string Name { get; set; }

    public UiAccessibilityControlType ControlType { get; set; }

    [JsonIgnore] public Rectangle BoundingRectangle { get; set; }

    [JsonIgnore] public bool IsOffscreen { get; set; }

    [JsonIgnore] public bool IsEnabled { get; set; }

    [JsonIgnore] public double ActualHeight { get; set; }

    [JsonIgnore] public double ActualWidth { get; set; }

    public bool IsDialog { get; set; }

    public string Id { get; set; }

    [JsonIgnore] public object Element { get; set; }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, ControlType, IsDialog);
    }
}