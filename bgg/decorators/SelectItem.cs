using Godot;
using System;

public class SelectItem : Node
{
    [Signal]
    public delegate void selection_changed(Boolean isSelected);

    [Signal]
    public delegate void item_event_occured(InputEvent ev);

    public Boolean is_busy { get; set; }

    public Boolean is_selected { get; private set; }

    public void select()
    {
        is_selected = true;
        EmitSignal(nameof(selection_changed), is_selected);
    }

    public void deselect()
    {
        is_selected = false;
        EmitSignal(nameof(selection_changed), is_selected);
    }

    public void accept_input(InputEvent ev)
    {
        EmitSignal(nameof(item_event_occured), ev);
    }
}