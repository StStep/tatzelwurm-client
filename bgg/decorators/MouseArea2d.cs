
using Godot;
using System;

public class MouseArea2d : Area2D
{
    [Signal]
    delegate void mouse_hover_changed();

    [Signal]
    delegate void event_while_hovering_occured(InputEvent ev);

    public Boolean is_mouse_hovering { get; private set; } = false;

    public override void _Ready()
    {
        SetProcessInput(false);
	    Connect("mouse_entered", this, "OnMouseEnter");
	    Connect("mouse_exited", this, "OnMouseExit");
	    Connect("visibility_changed", this, "OnVisibilityChange");
    }

    public override void _Input(InputEvent @event) => EmitSignal("event_while_hovering_occured", @event);

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
        EmitSignal("mouse_hover_changed");
    }

    // For when markers are manually moved
    private void MarkAsNotHovering()
    {
        is_mouse_hovering = false;
        SetProcessInput(false);
        EmitSignal("mouse_hover_changed");
    }
}