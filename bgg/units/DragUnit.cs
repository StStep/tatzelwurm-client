
using Godot;
using System;

public class DragUnit : Node2D
{
    [Signal]
    public delegate void Picked(DragUnit u);

    [Signal]
    public delegate void Placed(DragUnit u);

    [Signal]
    public delegate void Moved(DragUnit u);

    private Node _dragable;

    public Boolean CanDrag
    {
        get => (Boolean)_dragable.Get("can_drag");
        set => _dragable.Set("can_drag", value);
    }

    public Boolean Dragging
    {
        get => (Boolean)_dragable.Get("dragging");
        set => _dragable.Set("dragging", value);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _dragable = GetNode("Dragable");
	    _dragable.Connect("drag_started", this, nameof(Pickup));
	    _dragable.Connect("drag_ended", this, nameof(Place));
	    _dragable.Connect("point_to", this, nameof(PointTo));
	    _dragable.Connect("drag_to", this, nameof(MoveTo));
    }

    private void Pickup(Node dragable) => EmitSignal(nameof(Picked), this);
    private void Place(Node dragable) => EmitSignal(nameof(Placed), this);

    private void PointTo(float rads) => GlobalRotation = rads;
    private void MoveTo(Vector2 loc)
    {
        GlobalPosition = loc;
        EmitSignal(nameof(Moved), this);
    }
}
