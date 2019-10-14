using Godot;
using System;

public class SelectManager : Node
{
    public SelectItem SelectedItem { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        SetProcessInput(true);
    }

    public override void _Input(InputEvent @event) => SelectedItem?.AcceptInput(@event);

    public Boolean IsSelectionAllowed() => SelectedItem?.IsBusy != true;

    public void ReqSelection(SelectItem item)
    {
        if (item != SelectedItem && SelectedItem?.IsBusy != true)
        {
            SelectedItem?.Deselect();
            SelectedItem = item;
            SelectedItem?.Select();
        }
    }
}