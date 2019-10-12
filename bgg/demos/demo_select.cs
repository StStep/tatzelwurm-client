using Godot;
using System;

public class demo_select : Node
{
    public override void _Ready()
    {
        var selManager = GetNode("SelectManager") as Node;
        foreach (Node n in GetNode("Units").GetChildren())
        {
            n.Set("SelectManager", selManager);
        }
    }
}
