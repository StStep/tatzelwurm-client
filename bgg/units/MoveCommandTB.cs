using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MoveCommandTB : Control
{
    private float _yZero;
    private float _yMax;
    private float _xZero;
    private float _xMax;
    private Line2D _plot;
    private Label _xMinLabel;
    private Label _xMaxLabel;
    private Label _yMinLabel;
    private Label _yMaxLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _plot = GetNode<Line2D>("ViewportContainer/Viewport/Plot");
        _xMinLabel = GetNode<Label>("XminLabel");
        _xMaxLabel = GetNode<Label>("XmaxLabel");
        _yMinLabel = GetNode<Label>("YminLabel");
        _yMaxLabel = GetNode<Label>("YmaxLabel");
        var axis = GetNode<Line2D>("ViewportContainer/Viewport/Axis");
        _yMax = axis.Points[0].y;
        _yZero = axis.Points[1].y;
        _xZero = axis.Points[1].x;
        _xMax = axis.Points[2].x;

        // Test Values
        var pnts = new List<Vector2>
        {
            new Vector2(0f, 0f),
            new Vector2(25f, 25f),
            new Vector2(50f, 25f),
            new Vector2(75f, 75f),
            new Vector2(100f, 75f),
        };
        Plot(pnts, new Vector2(0f, 100f), new Vector2(0f, 100f));
    }

    public void Plot(IEnumerable<Vector2> points, Vector2 xRange, Vector2 yRange)
    {
       _plot.ClearPoints();
       _xMinLabel.Text = xRange[0].ToString();
       _xMaxLabel.Text = xRange[1].ToString();
       _yMinLabel.Text = yRange[0].ToString();
       _yMaxLabel.Text = yRange[1].ToString();
       foreach (var p in points)
       {
           var xWeight = (p.x - xRange[0])/(xRange[1] - xRange[0]);
           var yWeight = (p.y - yRange[0])/(yRange[1] - yRange[0]);
           var x = Mathf.Lerp(_xZero, _xMax, xWeight);
           var y = Mathf.Lerp(_yZero, _yMax, yWeight);
           _plot.AddPoint(new Vector2(x, y));
       }
    }
}
