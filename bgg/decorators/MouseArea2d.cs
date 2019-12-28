
using Godot;
using System;

public enum MouseButton {None, Left, Right, Middle};

public class MouseArea2d : Area2D
{
    [Signal]
    public delegate void mouse_hover_changed();

    [Signal]
    public delegate void mouse_drag_updated(int index, Vector2 start, Vector2 end, MouseButton button);

    [Signal]
    public delegate void mouse_drag_ended(int index, Vector2 start, Vector2 end, MouseButton button);

    [Signal]
    public delegate void mouse_clicked(MouseButton button);

    private int dragIndex = 0;
    private Vector2 dragStart = Vector2.Zero;
    private Boolean dragInProgress = false;
    private int currentButton = 0;
    private bool buttonPressed = false;

    public Boolean is_mouse_hovering { get; private set; } = false;

    public override void _Ready()
    {
        base._Ready();
        SetProcessInput(false);
	    Connect("mouse_entered", this, nameof(OnMouseEnter));
	    Connect("mouse_exited", this, nameof(OnMouseExit));
	    Connect("visibility_changed", this, nameof(OnVisibilityChange));
    }

    public override void _Input(InputEvent @event)
    {
        var mbutEvent = @event as InputEventMouseButton;
        if (mbutEvent != null)
        {
            var bDownAndUp = false;
            if (mbutEvent.ButtonIndex == currentButton && buttonPressed && !mbutEvent.Pressed)
            {
                bDownAndUp = true;
                buttonPressed = false;
            }
            else if (is_mouse_hovering && mbutEvent.Pressed)
            {
                currentButton = mbutEvent.ButtonIndex;
                buttonPressed = true;
            }
            else
            {
                buttonPressed = false;
            }

            if (buttonPressed)
            {
                dragStart = mbutEvent.GlobalPosition;
            }

            if (bDownAndUp && is_mouse_hovering && !dragInProgress)
            {
                EmitSignal(nameof(mouse_clicked), ButtonIndexToEnum(currentButton));
            }
            else if (bDownAndUp && dragInProgress)
            {

                EmitSignal(nameof(mouse_drag_ended), dragIndex, dragStart, mbutEvent.GlobalPosition, ButtonIndexToEnum(currentButton));
                dragInProgress = false;
                dragIndex++;
                SetProcessInput(false);
            }
        }

        var mmotEvent = @event as InputEventMouseMotion;
        if (mmotEvent != null && buttonPressed && (!is_mouse_hovering || dragInProgress))
        {
            dragInProgress = true;
            EmitSignal(nameof(mouse_drag_updated), dragIndex, dragStart, mmotEvent.GlobalPosition, ButtonIndexToEnum(currentButton));
        }
    }

    private void OnMouseEnter() => MarkAsHovering();
    private void OnMouseExit() => MarkAsNotHovering();

    private void OnVisibilityChange()
    {
        if (!Visible && is_mouse_hovering)
        {
            MarkAsNotHovering();
        }
    }

    private void MarkAsHovering()
    {
        is_mouse_hovering = true;
        SetProcessInput(true);
        EmitSignal(nameof(mouse_hover_changed));
    }

    private void MarkAsNotHovering()
    {
        is_mouse_hovering = false;
        if (!buttonPressed)
        {
            SetProcessInput(false);
        }
        EmitSignal(nameof(mouse_hover_changed));
    }

    private static MouseButton ButtonIndexToEnum(int key)
    {
        switch ((Godot.ButtonList)key)
        {
            case ButtonList.Left:
                return MouseButton.Left;
            case ButtonList.Right:
                return MouseButton.Right;
            case ButtonList.Middle:
                return MouseButton.Middle;
            default:
                return MouseButton.None;
        }
    }
}
