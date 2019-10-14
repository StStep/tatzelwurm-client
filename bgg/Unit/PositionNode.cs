using Godot;
using System;

public class PositionNode : Node2D
{
    private Color colNotHighlighted = new Color("ffffff"); // White
    private Color colHighlighted = new  Color("b6ff00"); // Green-Yellow

    private Vector2 _moveVector = Vector2.Zero;

    public MoveUnit ParentUnit { get; set; }
    public Vector2 StartPos => Previous != null ? Previous.EndPos : ParentUnit.GlobalPosition;

    public Vector2 EndPos
    {
        get => ToGlobal(Vector2.Zero);
        set
        {
            _moveVector = value - StartPos;
            GlobalPosition = value;
            GlobalRotation = _moveVector.Angle() + (float)(Mathf.Pi/2.0);
            PropogateEndPosUpdate();
        }
    }

    public MouseArea2d Marker;
    public Line2D Path;
    public MouseArea2d PathArea;

    public PositionNode Previous { get; set; }
    public PositionNode Next { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Marker = GetNode<MouseArea2d>("Marker");
        Path = GetNode<Line2D>("Path");
        PathArea = GetNode<MouseArea2d>("PathArea");

	    Marker.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnEvent));
	    Marker.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange));
	    PathArea.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnPathHovorChange));
    }

    private void OnEvent(InputEvent ev)
    {
        if (!ParentUnit.IsBusy)
        {
            ParentUnit.AdjustingNode = this;
            if (ParentUnit.HandleInput(ev))
            {
                GetTree().SetInputAsHandled();
            }
        }
    }

    private void OnMarkerHoverChange()
    {
        if (ParentUnit.IsSelected && !ParentUnit.IsBusy && Marker.is_mouse_hovering)
        {
            Marker.GetNode<Sprite>("Sprite").Modulate = colHighlighted;
        }
        else
        {
            Marker.GetNode<Sprite>("Sprite").Modulate = colNotHighlighted;
        }
    }

    private void  OnPathHovorChange()
    {
        if (ParentUnit.IsSelected && !ParentUnit.IsBusy && PathArea.is_mouse_hovering)
        {
            ParentUnit.HighlightedPathNode = this;
        }
    }

    private void PropogateEndPosUpdate()
    {
        GlobalPosition = StartPos + _moveVector;
        var l_vec = ToLocal(StartPos);
        Path.Points = new Vector2[] { l_vec, Vector2.Zero };
        var shape = new RectangleShape2D();
        shape.Extents = new Vector2(20, l_vec.Length()/2.0f);
        var shapeNode = GetNode<CollisionShape2D>("PathArea/Shape");
        shapeNode.SetShape(shape);
        shapeNode.Rotation = l_vec.Angle() + Mathf.Pi/2.0f;
        shapeNode.Position = new Vector2(l_vec.x/2.0f, l_vec.y/2.0f);
        if (Next != null)
        {
            Next.PropogateEndPosUpdate();
        }
    }

    public void Enable()
    {
        Marker.Show();
        SetProcessInput(true);
    }

    public void Disable()
    {
        Marker.Hide();
        SetProcessInput(false);
    }

    public void Erase()
    {
        if (Next != null)
        {
            Next.Erase();
        }
        QueueFree();
    }

    public Vector2 GetClosestPointOnPath(Vector2 gpos)
    {
        var pnt = ToLocal(gpos);
        var dir = ToLocal(StartPos).Normalized();
        var dot = pnt.Dot(dir);
        return ToGlobal(dir*dot);
    }

}
