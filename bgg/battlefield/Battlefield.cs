using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Battlefield : Node
{
    private PackedScene _deployUnitScene = GD.Load<PackedScene>("res://units/DragUnit.tscn");
    private PackedScene _moveUnitScene = GD.Load<PackedScene>("res://units/MoveUnit.tscn");

    private Boolean _deploying = true;
    private List<DragUnit> _deployUnits = new List<DragUnit>();
    private List<MoveUnit> _moveUnits = new List<MoveUnit>();
    private Area2D _deployZone;
    private Area2D _enemyDeployZone;
    private Area2D _battleZone;
    private Area2D _outBoundsZone;
    private Area2D _neutralZone;
    private Node2D _deployMarker;
    private Node2D _enemyDeployMarker;

    public Action<Boolean> ValidityChanged;
    public Boolean IsValid => (_deploying) ? !_deployUnits.Any(u => !u.Valid) : !_moveUnits.Any(u => !u.Valid);

    private Boolean __busy = false;
    public Action<Boolean> BusyChanged;
    public Boolean Busy
    {
        get => __busy;
        private set
        {
            if (__busy != value)
            {
                __busy = value;
                BusyChanged?.Invoke(__busy);
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _deployZone = GetNode<Area2D>("DeployZone");
        _enemyDeployZone = GetNode<Area2D>("EnemyDeployZone");
        _battleZone = GetNode<Area2D>("BattleZone");
        _outBoundsZone = GetNode<Area2D>("OutBoundsZone");
        _neutralZone = GetNode<Area2D>("NeutralZone");
        _deployMarker = GetNode<Node2D>("DeployMarker");
        _enemyDeployMarker = GetNode<Node2D>("EnemyDeployMarker");
        _deployMarker.Visible = true;
        _enemyDeployMarker.Visible = true;
    }

    public void DeployUnit(String type)
    {
        Busy = true;
        var u = _deployUnitScene.Instance() as DragUnit;
        GetNode("Units").AddChild(u);
        u.CanDrag = true;
        u.Dragging = true;
        u.Picked += PickedUnit;
        u.Placed += PlacedUnit;
        u.Moved += ValidateUnit;
        _deployUnits.Add(u);
    }

    public void Deploy2Move()
    {
        var selMan = GetNode<SelectManager>("Units");
        _deployMarker.Visible = false;
        _enemyDeployMarker.Visible = false;
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
        _deploying = false;
    }

    private void PickedUnit(DragUnit unit)
    {
        Busy = true;
        _deployUnits.ForEach(u => u.CanDrag = false);
        unit.CanDrag = true;
    }

    private void PlacedUnit(DragUnit unit)
    {
        _deployUnits.ForEach(u => u.CanDrag = true);
        Busy = false;
    }

    private void ValidateUnit(IUnit unit)
    {
        var bvalid = unit.Valid;
        if (unit.OverlapsArea(_deployZone) &&
            !unit.OverlapsArea(_outBoundsZone) &&
            !unit.OverlapsArea(_neutralZone))
        {
            unit.Modulate = Colors.White;
            unit.Valid = true;
        }
        else
        {
            unit.Modulate = Colors.Red;
            unit.Valid = false;
        }

        if (bvalid != unit.Valid)
            ValidityChanged?.Invoke(this.IsValid);
    }
}
