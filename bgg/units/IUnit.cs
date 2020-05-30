using Godot;
using System;

public interface IUnit
{
    Boolean Valid { get; set; }
    Color Modulate { get; set; }

    Boolean OverlapsArea(Area2D area);
}