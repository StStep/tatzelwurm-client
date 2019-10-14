using Godot;
using System;

public class demo_select : Node
{
    public override void _Ready()
    {
        var selManager = GetNode<SelectManager>("SelectManager");
        foreach (MoveUnit u in GetNode("Units").GetChildren())
        {
            u.SelectManager = selManager;
        }
    }
}
