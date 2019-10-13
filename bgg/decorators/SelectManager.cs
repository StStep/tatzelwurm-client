using Godot;
using System;

public class SelectManager : Node
{

    public SelectItem selected_item { get; private set; }

    public override void _Ready()
    {
        base._Ready();
        SetProcessInput(true);
    }

    public override void _Input(InputEvent @event) => selected_item?.accept_input(@event);

    public Boolean is_selection_allowed() => selected_item == null || !selected_item.is_busy;

    public void req_selection(SelectItem item)
    {
        if (item == null)
        {
            ClearSelection();
        }
        else if (item != selected_item)
        {
            ClearSelection();
            selected_item = item;
            item.select();
        }
        else { }
    }

    private void ClearSelection()
    {
        selected_item?.deselect();
        selected_item = null;
    }
}