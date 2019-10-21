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

    public MoveUnit ParentUnit { get; set; }
    public Vector2 StartPos => Previous != null ? Previous.GlobalPosition : ParentUnit.GlobalPosition;
    public float StartRot => Previous != null ? Previous.GlobalRotation : ParentUnit.GlobalRotation;

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

    public void SetAsLine(Vector2 end)
    {
        GlobalPosition = end;
        GlobalRotation = (end - StartPos).Angle() + (float)(Mathf.Pi/2.0);
        Path.Points = new Vector2[] { ToLocal(StartPos), Vector2.Zero };
        PathPoly.Polygon = Trig.GetLineAsPolygon(Path.Points, PATH_AREA_WIDTH);
    }

    public void SetAsArc(Vector2 end)
    {
        var a = new Trig.Arc2(new Trig.Ray2(StartPos, StartRot), end);
        GlobalPosition = end;
        GlobalRotation = a.EndDir.Angle() - (float)(Mathf.Pi/2.0);
        var pnts = new List<Vector2>();
        var seg_num = 20;
        var seg = a.Length/seg_num;
        for (int i = 0; i < seg_num; i++)
        {
            pnts.Add(ToLocal(a.GetPoint(seg * i)));
        }

        Path.Points = pnts.ToArray();
        PathPoly.Polygon = Trig.GetLineAsPolygon(pnts.ToArray(), PATH_AREA_WIDTH);
    }

    public void highlight_body(String type)
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

    public void highlight_path(String type, float coverage)
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

    public Vector2 GetClosestPointOnPath(Vector2 gpos)
    {
        var pnt = ToLocal(gpos);
        var dir = ToLocal(StartPos).Normalized();
        var dot = pnt.Dot(dir);
        return ToGlobal(dir*dot);
    }

}
