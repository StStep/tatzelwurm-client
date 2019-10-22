using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattlefieldView : Node
{
    private PackedScene _moveUnitScene = GD.Load<PackedScene>("res://units/MoveUnit.tscn"); // Will load when the script is instanced.
    private List<MoveUnit> _units = new List<MoveUnit>();
    private SelectManager _selManager = new SelectManager();

    public override void _Ready()
    {
        base._Ready();
    }

    // Remove all previously added units and commands
    public void clear()
    {
        _units.ForEach(u => u.ResetMoveNodes());
        _units.ForEach(u => u.QueueFree());
        _units.Clear();
    }

    // Add a unit to be rendered
    //
    // * ref - (String) Unit reference to place the new unit under
    // * gpos - (Vector2) Global position for unit on battlefield
    // * gdir - (Vector2) A vector pointing in a global direction to face the representation
    public int new_unit(Vector2 gpos, Vector2 gdir)
    {
        // Unit Block
        var inst = _moveUnitScene.Instance() as MoveUnit;
        inst.SelectManager = _selManager;
        AddChild(inst);
        inst.GlobalPosition = gpos;
        inst.GlobalRotation = gdir.Angle() + Mathf.Pi/2;
        _units.Add(inst);
        inst.SetMoveIndicatorVisibility(false);

        return _units.Count - 1;
    }

    public void AddMove(int ind, bool arc, Vector2 gpos)
        => AddMove(ind, arc, gpos, Enumerable.Empty<String>());

    public void AddMove(int ind, bool arc, Vector2 gpos, Vector2 gdir)
        => AddMove(ind, arc, gpos, gdir, Enumerable.Empty<String>());

    public void AddMove(int ind, bool arc, Vector2 gpos, IEnumerable<String> annotations)
    {
        _units[ind].AddMoveNode(gpos, arc, annotations);
    }

    public void AddMove(int ind, bool arc, Vector2 gpos, Vector2 gdir, IEnumerable<String> annotations)
    {
        throw new NotImplementedException();
    }

    // Enable/Disable the end-of-turn movement indicator for a unit
    //
    // * ref - (String) Unit reference to modify
    // * en - (Vector2) True if the indicator should be displayed, otherwise false
    public void display_eot_move(int ind, bool en)
    {
        _units[ind].SetMoveIndicatorVisibility(en);
    }
}