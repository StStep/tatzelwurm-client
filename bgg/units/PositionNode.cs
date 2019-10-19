using Godot;
using System;
using System.Collections.Generic;

public class PositionNode : Node2D
{
    const int PATH_AREA_WIDTH = 10;

    private Color colNotHighlighted = new Color("ffffff"); // White
    private Color colHighlighted = new  Color("b6ff00"); // Green-Yellow
    private Color colInvalid = new  Color("e2342b");
    private Color colInavtive = new  Color("b2b2b2");

    Dictionary<String, Node2D> _annotations;

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

    public MouseArea2d Body;
    public CollisionShape2D BodyShape;
    public Line2D Path;
    public MouseArea2d PathArea;
    public CollisionPolygon2D PathPoly;
    public Sprite Sprite;

    public PositionNode Previous { get; set; }
    public PositionNode Next { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Body = GetNode<MouseArea2d>("BodyArea");
        BodyShape = Body.GetNode<CollisionShape2D>("Shape");
        Path = GetNode<Line2D>("Path");
        PathArea = GetNode<MouseArea2d>("PathArea");
        PathPoly = PathArea.GetNode<CollisionPolygon2D>("Polygon");
        Sprite = GetNode<Sprite>("Sprite");

        _annotations = new Dictionary<string, Node2D>() {
            { "reposition" , GetNode<Node2D>("Reposition") },
            { "wheel" , GetNode<Node2D>("Wheel") },
            { "rotation" , GetNode<Node2D>("Rotation") },
        };

        Body.Connect(nameof(MouseArea2d.event_while_hovering_occured), this, nameof(OnEvent));
        Body.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange));
        PathArea.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnPathHovorChange));
        clear_annotations();
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
        if (ParentUnit.IsSelected && !ParentUnit.IsBusy && Body.is_mouse_hovering)
        {
            Sprite.Modulate = colHighlighted;
        }
        else
        {
            Sprite.Modulate = colNotHighlighted;
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
        BodyShape.SetShape(shape);
        BodyShape.Rotation = l_vec.Angle() + Mathf.Pi/2.0f;
        BodyShape.Position = new Vector2(l_vec.x/2.0f, l_vec.y/2.0f);
        if (Next != null)
        {
            Next.PropogateEndPosUpdate();
        }
    }

    public void display_cmd(bool en)
    {
        Sprite.Visible = en;
    }

    public void clear_annotations()
    {
        foreach (var n in _annotations)
        {
            n.Value.Hide();
        }
        display_cmd(false);
    }

    public void add_annotation(String uref)
    {
        _annotations[uref].Show();
    }

    public void clear_path()
    {
        Path.Points = new Vector2[] {};
        PathPoly.Polygon = new Vector2[] {};
    }

    private void  set_path_as_line(Vector2 start)
    {
        Path.Points = new Vector2[] { ToLocal(start), Vector2.Zero };
        PathPoly.Polygon = Trig.GetLineAsPolygon(Path.Points, PATH_AREA_WIDTH);
    }

    private void set_path_as_arc(Vector2 start, Vector2 start_dir)
    {
        var a = new Trig.Arc2(new Trig.Ray2(start, start_dir), ToGlobal(Vector2.Zero));
        var pnts = new List<Vector2>();
        var seg_num = 20;
        var seg = a.Length/seg_num;
        for (int i = 0; i < seg_num; i++)
            pnts.Add(ToLocal(a.GetPoint(seg * i)));
        Path.Points = pnts.ToArray();
        PathPoly.Polygon = Trig.GetLineAsPolygon(pnts.ToArray(), PATH_AREA_WIDTH);
    }

    private void highlight_body(String type)
    {
        if (type == "None")
        {
            Sprite.Modulate = colNotHighlighted;
        }
        else if (type == "Focus")
        {
            Sprite.Modulate = colNotHighlighted;
        }
        else if (type == "Invalid")
        {
            Sprite.Modulate = colInvalid;
        }
        else if (type == "Inactive")
        {
            Sprite.Modulate = colInavtive;
        }
        else
        {
        }
    }

    private void highlight_path(String type, float coverage)
    {

    }

    public void Enable()
    {
        display_cmd(true);
        SetProcessInput(true);
    }

    public void Disable()
    {
        display_cmd(false);
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
