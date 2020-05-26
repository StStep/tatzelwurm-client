using Godot;
using System;
using System.Collections.Generic;

public class Battlefield : Node
{
    private PackedScene _deployUnitScene = GD.Load<PackedScene>("res://units/DragUnit.tscn");
    private PackedScene _moveUnitScene = GD.Load<PackedScene>("res://units/MoveUnit.tscn");

    private List<DragUnit> _deployUnits = new List<DragUnit>();
    private List<MoveUnit> _moveUnits = new List<MoveUnit>();
    private Area2D _deployZone1;
    private Area2D _deployZone2;
    private Area2D _battleZone;
    private Area2D _outBoundsZone;
    private Area2D _neutralZone;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _deployZone1 = GetNode<Area2D>("DeployZone1");
        _deployZone2 = GetNode<Area2D>("DeployZone2");
        _battleZone = GetNode<Area2D>("BattleZone");
        _outBoundsZone = GetNode<Area2D>("OutBoundsZone");
        _neutralZone = GetNode<Area2D>("NeutralZone");
    }

    public void DeployUnit(String type)
    {
        var u = _deployUnitScene.Instance() as DragUnit;
        GetNode("Units").AddChild(u);
        u.CanDrag = true;
        u.Dragging = true;
        u.Connect(nameof(DragUnit.Picked), this, nameof(PickedUnit));
        u.Connect(nameof(DragUnit.Placed), this, nameof(PlacedUnit));
        u.Connect(nameof(DragUnit.Moved), this, nameof(ValidateUnit));
        _deployUnits.Add(u);
    }

    public void Deploy2Move()
    {
        var selMan = GetNode<SelectManager>("Units");
        _deployZone1.Visible = false;
        _deployZone2.Visible = false;
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

    private void ValidateUnit(DragUnit unit)
    {
        if (unit.OverlapsArea(_deployZone1) &&
            !unit.OverlapsArea(_outBoundsZone) &&
            !unit.OverlapsArea(_neutralZone))
        {
            unit.Modulate = Colors.White;
        }
        else
        {
            unit.Modulate = Colors.Red;
        }
    }
}
