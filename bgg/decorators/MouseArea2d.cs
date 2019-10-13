
using Godot;
using System;

public class MouseArea2d : Area2D
{
    [Signal]
    public delegate void mouse_hover_changed();

    [Signal]
    public delegate void event_while_hovering_occured(InputEvent ev);

    public Boolean is_mouse_hovering { get; private set; } = false;

    public override void _Ready()
    {
        base._Ready();
        SetProcessInput(false);
	    Connect("mouse_entered", this, nameof(OnMouseEnter));
	    Connect("mouse_exited", this, nameof(OnMouseExit));
	    Connect("visibility_changed", this, nameof(OnVisibilityChange));
    }

    public override void _Input(InputEvent @event) => EmitSignal(nameof(event_while_hovering_occured), @event);

    private void OnMouseEnter() => MarkAsHovering();
    private void OnMouseExit() => MarkAsNotHovering();

    private void OnVisibilityChange()
    {
        if (!Visible && is_mouse_hovering)
        {
            MarkAsNotHovering();
        }
    }

    // For when markers are manually moved
    private void MarkAsHovering()
    {
        is_mouse_hovering = true;
        SetProcessInput(true);
        EmitSignal(nameof(mouse_hover_changed));
    }

    // For when markers are manually moved
    private void MarkAsNotHovering()
    {
        is_mouse_hovering = false;
        SetProcessInput(false);
        EmitSignal(nameof(mouse_hover_changed));
    }
}
