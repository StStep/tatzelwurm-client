using Godot;
using System;

public class PositionNode : Node2D
{
    public Unit unit { get; set; }
    public Vector2 start => previous != null ? previous.end : unit.GlobalPosition;

    public Vector2 end
    {
        get => ToGlobal(Vector2.Zero);
        set
        {
            move = value - start;
            GlobalPosition = value;
            GlobalRotation = move.Angle() + (float)(Mathf.Pi/2.0);
            update();
        }
    }

    public Color C_NOT_HIGHLIGHTED = new Color("ffffff"); // White
    public Color C_HIGHLIGHT = new  Color("b6ff00"); // Green-Yellow

    public MouseArea2d marker;
    public Line2D path;
    public MouseArea2d path_area;
    public CollisionShape2D path_shape;

    public PositionNode previous = null;
    public PositionNode next = null;

    public Vector2 move = Vector2.Zero;

    public override void _Ready()
    {
        base._Ready();
        marker = GetNode<MouseArea2d>("Marker");
        path = GetNode<Line2D>("Path");
        path_area = GetNode<MouseArea2d>("PathArea");
        path_shape = GetNode<CollisionShape2D>("PathArea/Shape");

	    marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(_render_marker_highlight));
	    marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(_accept_marker_event));
	    path_area.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(_render_path_highlight));
    }

    private void _accept_marker_event(InputEvent ev)
    {
        if (!unit.is_busy)
        {
            unit.mv_adj = this;
            if (unit.handle_input(ev))
            {
                GetTree().SetInputAsHandled();
            }
        }
    }

    private void _render_marker_highlight()
    {
        if (unit.is_selected && !unit.is_busy && marker.is_mouse_hovering)
        {
            marker.GetNode<Sprite>("Sprite").Modulate = C_HIGHLIGHT;
        }
        else
        {
            marker.GetNode<Sprite>("Sprite").Modulate = C_NOT_HIGHLIGHTED;
        }
    }

    private void  _render_path_highlight()
    {
        if (unit.is_selected && !unit.is_busy && path_area.is_mouse_hovering)
        {
            unit.high_path = this;
        }
    }

    public void enable()
    {
        marker.Show();
        SetProcessInput(true);
    }

    public void disable()
    {
        marker.Hide();
        SetProcessInput(false);
    }

    public void update()
    {
        GlobalPosition = start + move;
        var l_vec = ToLocal(start);
        path.Points = new Vector2[] { l_vec, Vector2.Zero };
        var shape = new RectangleShape2D();
        shape.Extents = new Vector2(20, l_vec.Length()/2.0f);
        path_shape.SetShape(shape);
        path_shape.Rotation = l_vec.Angle() + Mathf.Pi/2.0f;
        path_shape.Position = new Vector2(l_vec.x/2.0f, l_vec.y/2.0f);
        if (next != null)
            next.update();
    }

    public void erase()
    {
        if (next != null)
            next.erase();
        QueueFree();
    }

    public Vector2 closest_pnt_on_path(Vector2 gpos)
    {
        var pnt = ToLocal(gpos);
        var dir = ToLocal(start).Normalized();
        var dot = pnt.Dot(dir);
        return ToGlobal(dir*dot);
    }

}
