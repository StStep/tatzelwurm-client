
using Godot;
using System;

public class DragUnit : Node2D, IUnit
{
    public Action<DragUnit> Picked;
    public Action<DragUnit> Placed;
    public Action<DragUnit> Moved;

    private Area2D _dragable;

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

    public Boolean Valid { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _dragable = GetNode<Area2D>("Dragable");
	    _dragable.Connect("drag_started", this, nameof(Pickup));
	    _dragable.Connect("drag_ended", this, nameof(Place));
	    _dragable.Connect("point_to", this, nameof(PointTo));
	    _dragable.Connect("drag_to", this, nameof(MoveTo));
    }

    public Boolean OverlapsArea(Area2D area) => _dragable.OverlapsArea(area);

    private void Pickup(Node dragable) => Picked?.Invoke(this);
    private void Place(Node dragable) => Placed?.Invoke(this);

    private void PointTo(float rads)
    {
        GlobalRotation = rads;
        Moved?.Invoke(this);
    }

    private void MoveTo(Vector2 loc)
    {
        GlobalPosition = loc;
        Moved?.Invoke(this);
    }
}
