using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Plot : Control
{
    private float _yZero;
    private float _yMax;
    private float _xZero;
    private float _xMax;

    private Line2D _xgrid;
    private Line2D _ygrid;
    private Line2D _plot;
    private Line2D _target;
    private Line2D _current;
    private Label _title;
    private Label _xMinLabel;
    private Label _xMaxLabel;
    private Label _yMinLabel;
    private Label _yMaxLabel;
    private Label _xAxisLabel;
    private Label _yAxisLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _xgrid = GetNode<Line2D>("ViewportContainer/Viewport/Xgrid");
        _ygrid = GetNode<Line2D>("ViewportContainer/Viewport/Ygrid");
        _plot = GetNode<Line2D>("ViewportContainer/Viewport/Plot");
        _target = GetNode<Line2D>("ViewportContainer/Viewport/Target");
        _current = GetNode<Line2D>("ViewportContainer/Viewport/Current");
        _title = GetNode<Label>("Title");
        _xMinLabel = GetNode<Label>("XminLabel");
        _xMaxLabel = GetNode<Label>("XmaxLabel");
        _yMinLabel = GetNode<Label>("YminLabel");
        _yMaxLabel = GetNode<Label>("YmaxLabel");
        _xAxisLabel = GetNode<Label>("XaxisLabel");
        _yAxisLabel = GetNode<Label>("YaxisLabel");

        // Update axis on resize
        var vp = GetNode<Viewport>("ViewportContainer/Viewport");
        vp.Connect("size_changed", this, nameof(this.Resized));
        Resized();

    }

    public void Resized()
    {
        var vp = GetNode<Viewport>("ViewportContainer/Viewport");
        var axis = vp.GetNode<Line2D>("Axis");
        axis.ClearPoints();
        axis.AddPoint(new Vector2(0, 0));
        axis.AddPoint(new Vector2(0, vp.Size.y));
        axis.AddPoint(new Vector2(vp.Size.x, vp.Size.y));
        _yMax = axis.Points[0].y;
        _yZero = axis.Points[1].y;
        _xZero = axis.Points[1].x;
        _xMax = axis.Points[2].x;
        Reset();
    }

    public void Reset()
    {
        var errscreen = GetNode<Control>("Error");
        errscreen.Hide();

        // Test Values
        var xrange = new Vector2(0f, 10f);
        var yrange = new Vector2(0f, 100f);
        var pnts = new List<Vector2>
        {
            new Vector2(0f, 0f),
            new Vector2(2.5f, 25f),
            new Vector2(5f, 25f),
            new Vector2(7.5f, 75f),
            new Vector2(10f, 75f),
        };
        SetGrid(2.5f, 25f, xrange, yrange);
        SetTarget(75f, yrange);
        SetCurrent(5f, xrange);
        SetPlot("Example", pnts, xrange, yrange, "Time (s)", "Velocity (m/s)");
    }

    public void Error(String error)
    {
        var errscreen = GetNode<Control>("Error");
        var errtxt = errscreen.GetNode<Label>("Label");
        errtxt.Text = error;
        errscreen.Show();
    }

    public void SetPlot(String title, IEnumerable<Vector2> points, Vector2 xRange, Vector2 yRange, String xlabel, String ylabel)
    {
        _plot.ClearPoints();
        _title.Text = title;
        _xMinLabel.Text = xRange[0].ToString();
        _xMaxLabel.Text = xRange[1].ToString();
        _yMinLabel.Text = yRange[0].ToString();
        _yMaxLabel.Text = yRange[1].ToString();
        _xAxisLabel.Text = xlabel;
        _yAxisLabel.Text = ylabel;
        foreach (var p in points)
        {
            var xWeight = (p.x - xRange[0])/(xRange[1] - xRange[0]);
            var yWeight = (p.y - yRange[0])/(yRange[1] - yRange[0]);
            var x = Mathf.Lerp(_xZero, _xMax, xWeight);
            var y = Mathf.Lerp(_yZero, _yMax, yWeight);
            _plot.AddPoint(new Vector2(x, y));
        }
    }

    public void SetTarget(float ty, Vector2 yRange)
    {
        var yWeight = (ty - yRange[0])/(yRange[1] - yRange[0]);
        var y = Mathf.Lerp(_yZero, _yMax, yWeight);
        _target.ClearPoints();
        _target.AddPoint(new Vector2(_xZero, y));
        _target.AddPoint(new Vector2(_xMax, y));
    }

    public void SetCurrent(float tx, Vector2 xRange)
    {
        var xWeight = (tx - xRange[0])/(xRange[1] - xRange[0]);
        var x = Mathf.Lerp(_xZero, _xMax, xWeight);
        _current.ClearPoints();
        _current.AddPoint(new Vector2(x, _yZero));
        _current.AddPoint(new Vector2(x, _yMax));
    }

    public void SetGrid(float xspacing, float yspacing, Vector2 xRange, Vector2 yRange)
    {
        _xgrid.ClearPoints();
        for(float ix = xRange[0]; ix <= xRange[1]; ix += xspacing)
        {
            var xWeight = (ix - xRange[0])/(xRange[1] - xRange[0]);
            var x = Mathf.Lerp(_xZero, _xMax, xWeight);
            _xgrid.AddPoint(new Vector2(x, _yZero));
            _xgrid.AddPoint(new Vector2(x, _yMax));
            _xgrid.AddPoint(new Vector2(x, _yZero));
        }

        _ygrid.ClearPoints();
        for(float iy = yRange[0]; iy <= yRange[1]; iy += yspacing)
        {
            var yWeight = (iy - yRange[0])/(yRange[1] - yRange[0]);
            var y = Mathf.Lerp(_yZero, _yMax, yWeight);
            _ygrid.AddPoint(new Vector2(_xZero, y));
            _ygrid.AddPoint(new Vector2(_xMax, y));
            _ygrid.AddPoint(new Vector2(_xZero, y));
        }
    }
}
