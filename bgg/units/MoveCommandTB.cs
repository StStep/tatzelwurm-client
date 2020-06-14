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
    private Line2D _target;
    private Label _xMinLabel;
    private Label _xMaxLabel;
    private Label _yMinLabel;
    private Label _yMaxLabel;
    private Label _xAxisLabel;
    private Label _yAxisLabel;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _plot = GetNode<Line2D>("ViewportContainer/Viewport/Plot");
        _target = GetNode<Line2D>("ViewportContainer/Viewport/Target");
        _xMinLabel = GetNode<Label>("XminLabel");
        _xMaxLabel = GetNode<Label>("XmaxLabel");
        _yMinLabel = GetNode<Label>("YminLabel");
        _yMaxLabel = GetNode<Label>("YmaxLabel");
        _xAxisLabel = GetNode<Label>("XaxisLabel");
        _yAxisLabel = GetNode<Label>("YaxisLabel");
        var axis = GetNode<Line2D>("ViewportContainer/Viewport/Axis");
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
        var pnts = new List<Vector2>
        {
            new Vector2(0f, 0f),
            new Vector2(25f, 25f),
            new Vector2(50f, 25f),
            new Vector2(75f, 75f),
            new Vector2(100f, 75f),
        };
        Plot(pnts, new Vector2(0f, 100f), new Vector2(0f, 100f), "Time", "Velocity");
    }

    public void Error(String error)
    {
        var errscreen = GetNode<Control>("Error");
        var errtxt = errscreen.GetNode<Label>("Label");
        errtxt.Text = error;
        errscreen.Show();
    }

    public void Plot(IEnumerable<Vector2> points, Vector2 xRange, Vector2 yRange, String xlabel, String ylabel)
    {
       _plot.ClearPoints();
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

    public readonly Mobility Mobility = new Mobility()
    {
        MaxRotVelocity = Mathf.Pi / 5f,
        CwAcceleration = Mathf.Pi / 5f,
        CcwAcceleration = Mathf.Pi / 5f,
        Front = new DirectionalMobility()
        {
            MaxSpeed = 200f,
            Acceleration = 400f,
            Deceleration = 320f,
        },
        Back = new DirectionalMobility()
        {
            MaxSpeed = 100f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
        Left = new DirectionalMobility()
        {
            MaxSpeed = 100f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
        Right = new DirectionalMobility()
        {
            MaxSpeed = 100f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
    };

    public void PlotRotation(Boolean velocity)
    {
        var u = new MoveUnit();
        var period = 4f;
        var desRot = Mathf.Pi/2f;
        var yrange = new Vector2(0f, 2 * Mathf.Pi);
        var xrange = new Vector2(0f, period);
        var init = new MovementState()
        {
            Position = new Vector2(0f, 0f),
            Rotation = 0f,
            RotVelocity = 0f,
            Velocity = new Vector2(0f, 0f)
        };
        try
        {
            var testState = MoveCommand.MakeRotation(period, Mobility, init, desRot);
            GD.Print($"{testState.Preview.Count()} Entries, ends at Rot: {testState.Final.Rotation} Vrot: {testState.Final.RotVelocity} t: {testState.Preview.Last().Item1}");
            if (velocity)
            {
                SetTarget(0f, new Vector2(-Mathf.Pi, Mathf.Pi));
                Plot(testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.RotVelocity)), xrange, new Vector2(-Mathf.Pi, Mathf.Pi), "Time", "Rot. Velocity");
            }
            else
            {
                SetTarget(desRot, yrange);
                Plot(testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.Rotation)), xrange, yrange, "Time", "Rotation");
            }
        }
        catch(Exception ex)
        {
            Error(ex.Message);
        }
    }

    public void PlotApproach(Boolean velocity)
    {
        var u = new MoveUnit();
        var period = 4f;
        var desRotVel = Mobility.MaxRotVelocity;
        var yrange = new Vector2(-Mathf.Pi/2f, Mathf.Pi/2f);
        var xrange = new Vector2(0f, period);

        var st = new MovementState()
        {
            Position = new Vector2(0f, 0f),
            Rotation = 0f,
            RotVelocity = -Mobility.MaxRotVelocity,
            Velocity = new Vector2(0f, 0f)
        };
        try
        {
            if (velocity)
            {
                throw new NotImplementedException("Haven't implemented velocity approach");
            }
            else
            {
                SetTarget(desRotVel, yrange);
                var delta = period/100f;
                var _pnts = new List<Vector2>() { new Vector2(0f, st.RotVelocity) };
                for (var t = delta; t <= period; t += delta)
                {
                    st.RotVelocity = Mobility.ApproachRotVelocity(st.RotVelocity, desRotVel, delta);
                    _pnts.Add(new Vector2(t, st.RotVelocity));
                }
                Plot(_pnts, xrange, yrange, "Time", "Rot. Velocity");
            }
        }
        catch(Exception ex)
        {
            Error(ex.Message);
        }
    }
}
