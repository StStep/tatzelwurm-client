using Godot;
using System;

public class SelectItem : Node
{
    [Signal]
    public delegate void selection_changed(Boolean isSelected);

    public Boolean IsBusy { get; set; }

    public Boolean IsSelected { get; private set; }

    public void Select()
    {
        IsSelected = true;
        EmitSignal(nameof(selection_changed), IsSelected);
    }

    public void Deselect()
    {
        IsSelected = false;
        EmitSignal(nameof(selection_changed), IsSelected);
    }
}