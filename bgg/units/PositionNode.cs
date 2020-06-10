using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

using Trig;

public class PositionNode : Node2D
{
    const int PATH_AREA_WIDTH = 10;

    // TODO Get from unit?
    const float MAX_SPEED = 200f;

    [Signal]
    public delegate void clicked_on_hover(PositionNode node, MouseButton button);

    [Signal]
    public delegate void path_hover(Vector2 gpos);

    [Signal]
    public delegate void marker_hover(Position2D node, Boolean hovering);

    private Color colNotHighlighted = new Color("ffffff"); // White
    private Color colHighlighted = new  Color("b6ff00"); // Green-Yellow
    private Color colInvalid = new  Color("e2342b");
    private Color colInavtive = new  Color("b2b2b2");

    Dictionary<String, Node2D> _annotations;

    public MouseArea2d Body;
    public CollisionShape2D BodyShape;
    public Line2D Path;
    public MouseArea2d PathArea;
    public CollisionPolygon2D PathPoly;
    public Sprite Sprite;

    public PositionNode Previous { get; set; }
    public PositionNode Next { get; set; }

    private MoveCommand __Command;
    public MoveCommand Command
    {
        get => __Command;
        set
        {
            var p = value.PreviewPath(20);
            GlobalRotation = value.HeadingFunc.Invoke(value.Period);
            GlobalPosition = p.Last();
            // Do local conversoins after changing position
            Path.Points = p.Select(s => ToLocal(s)).ToArray();
            Path.Gradient = new Gradient();
            var baseColor = Colors.Red;
            var highColor = Colors.Green;
            foreach (var sp in value.PreviewSpeed(20))
            {
                GD.Print($"Len {sp[0]} Speed {sp[1]}");
                Path.Gradient.AddPoint(sp[0], baseColor.LinearInterpolate(highColor, sp[1]/MAX_SPEED));
            }
            Path.Gradient.RemovePoint(1);
            Path.Gradient.RemovePoint(0);
            PathPoly.Polygon = Utility.GetLineAsPolygon(Path.Points, PATH_AREA_WIDTH);
            __Command = value;
        }
    }

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

        Body.Connect(nameof(MouseArea2d.mouse_clicked), this, nameof(OnClick));
        Body.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnMarkerHoverChange));
        PathArea.Connect(nameof(MouseArea2d.mouse_hover_changed), this, nameof(OnPathHovorChange));
        clear_annotations();
    }

    private void OnClick(MouseButton button)
    {
        if (IsProcessingInput())
            EmitSignal(nameof(clicked_on_hover), this, button);
    }

    private void OnMarkerHoverChange()
    {
        if (IsProcessingInput())
            EmitSignal(nameof(marker_hover), this, Body.is_mouse_hovering);
    }

    private void OnPathHovorChange()
    {
        if (!IsProcessingInput())
        { }
        else if (PathArea.is_mouse_hovering)
        {
            EmitSignal(nameof(path_hover), PathArea.GetGlobalMousePosition());
        }
    }

    public void clear_annotations()
    {
        foreach (var n in _annotations)
        {
            n.Value.Hide();
        }
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


    public void highlight_body(String type)
    {
        if (type == "None")
        {
            Sprite.Modulate = colNotHighlighted;
        }
        else if (type == "Focus")
        {
            Sprite.Modulate = colHighlighted;
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
        SetProcessInput(true);
    }

    public void Disable()
    {
        SetProcessInput(false);
    }

    public Vector2 GetClosestPointOnPath(Vector2 startGpos, Vector2 gpos)
    {
        var pnt = ToLocal(gpos);
        var dir = ToLocal(startGpos).Normalized();
        var dot = pnt.Dot(dir);
        return ToGlobal(dir*dot);
    }

}
