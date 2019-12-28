using Godot;
using System;

public class test_mouse_area_2d : Node2D
{
    private MouseArea2d node_1;
    private MouseArea2d node_2;
    private int lastDragIndex_1 = -1;
    private int lastDragIndex_2 = -1;

    public override void _Ready()
    {
        SetProcess(true);
        node_1 = GetNode<MouseArea2d>("MouseArea2D_1");
        node_1.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnHoverChange), new Godot.Collections.Array() { 1 });
        node_1.Connect(nameof(MouseArea2d.mouse_drag_updated), this, nameof(OnDragUpdate), new Godot.Collections.Array() { 1 });
        node_1.Connect(nameof(MouseArea2d.mouse_drag_ended), this, nameof(OnDragStop), new Godot.Collections.Array() { 1 });
        node_1.Connect(nameof(MouseArea2d.mouse_clicked), this, nameof(OnMouseClick), new Godot.Collections.Array() { 1 });

        node_2 = GetNode<MouseArea2d>("MouseArea2D_2");
        node_2.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnHoverChange), new Godot.Collections.Array() { 2 });
        node_2.Connect(nameof(MouseArea2d.mouse_drag_updated), this, nameof(OnDragUpdate), new Godot.Collections.Array() { 2 });
        node_2.Connect(nameof(MouseArea2d.mouse_drag_ended), this, nameof(OnDragStop), new Godot.Collections.Array() { 2 });
        node_2.Connect(nameof(MouseArea2d.mouse_clicked), this, nameof(OnMouseClick), new Godot.Collections.Array() { 2 });
    }

    private void OnHoverChange(int id)
    {
        GD.Print($"{id} Hover is {(id == 1 ? node_1.is_mouse_hovering : node_2.is_mouse_hovering)}");
    }

    private void OnDragUpdate(int index, Vector2 start, Vector2 end, MouseButton button, int id)
    {
        if (id == 1)
        {
            if (lastDragIndex_1 != index)
            {
                GD.Print($"{id} Dragging from {start} with ind {index} and button {button}");
                lastDragIndex_1 = index;
            }
        }
        else
        {
            if (lastDragIndex_2 != index)
            {
                GD.Print($"{id} Dragging from {start} with ind {index} and button {button}");
                lastDragIndex_2 = index;
            }
        }
    }

    private void OnDragStop(int index, Vector2 start, Vector2 end, MouseButton button, int id)
    {
        GD.Print($"{id} Stopped dragging from {start} to {end} with ind {index} and button {button}");
    }

    private void OnMouseClick(MouseButton button,int id)
    {
        GD.Print($"{id} Clicked button {button}");
    }
}
