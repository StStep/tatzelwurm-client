using Godot;
using System;
using System.Collections.Generic;

public class Battlefield : Node
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    private PackedScene _deployUnitScene = GD.Load<PackedScene>("res://units/DragUnit.tscn");
    private PackedScene _moveUnitScene = GD.Load<PackedScene>("res://units/MoveUnit.tscn");

    private List<DragUnit> _deployUnits = new List<DragUnit>();
    private List<MoveUnit> _moveUnits = new List<MoveUnit>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    public void DeployUnit(String type)
    {
        var u = _deployUnitScene.Instance() as DragUnit;
        GetNode("Units").AddChild(u);
        u.CanDrag = true;
        u.Dragging = true;
        u.Connect("Picked", this, nameof(PickedUnit));
        u.Connect("Placed", this, nameof(PlacedUnit));
        _deployUnits.Add(u);
    }

    public void Deploy2Move()
    {
        var selMan = GetNode<SelectManager>("Units");
        foreach (var u in _deployUnits)
        {
            var newUnit = _moveUnitScene.Instance() as MoveUnit;
            newUnit.SelectManager = selMan;
            newUnit.GlobalPosition = u.GlobalPosition;
            newUnit.GlobalRotation = u.GlobalRotation;
            _moveUnits.Add(newUnit);
            GetNode("Units").AddChild(newUnit);
            u.QueueFree();
        }
        _deployUnits.Clear();
    }

    private void PickedUnit(DragUnit unit)
    {
        _deployUnits.ForEach(u => u.CanDrag = false);
        unit.CanDrag = true;
    }

    private void PlacedUnit(DragUnit unit)
    {
        _deployUnits.ForEach(u => u.CanDrag = true);
    }
}
