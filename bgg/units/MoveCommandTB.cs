using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class MoveCommandTB : Control
{
    Plot posPlot;
    Plot velPlot;
    MoveAnimator moveAnim;
    Label tLabel;
    Slider tSlider;

    Vector2 rangeT;
    float curT;
    Boolean playing;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        posPlot = GetNode<Plot>("PositionPlot");
        velPlot = GetNode<Plot>("VelocityPlot");
        moveAnim = GetNode<MoveAnimator>("MoveAnimator");
        tLabel = GetNode<Label>("CurrentT");
        tSlider = GetNode<Slider>("TimeSlider");

        // Start with Rotation Plot
        PlotRotation();
    }

    public override void _Process(float delta)
    {
        if (playing)
        {
            SetT(curT + delta);
        }
    }

    public void PlayPause(bool playing)
    {
        this.playing = playing;
        tSlider.Editable = !playing;
    }

    public void OnSliderSet(float value)
    {
        if (playing)
            return;

        SetT(value/100f);
    }

    public void SetT(float t)
    {
        curT = t > rangeT[1] ? rangeT[0] : t;
        velPlot.SetCurrent(curT, rangeT);
        posPlot.SetCurrent(curT, rangeT);
        tLabel.Text = $"{curT:0.00}/{rangeT[1]:0.00} sec";
        tSlider.Value = t * 100f;
        tSlider.MinValue = rangeT[0] * 100f;
        tSlider.MaxValue = rangeT[1] * 100f;
        moveAnim.SetT(curT);
    }

    public readonly Mobility Mobility = new Mobility()
    {
        MaxRotVelocity = Mathf.Pi / 5f,
        CwAcceleration = 2f*Mathf.Pi / 5f,
        CcwAcceleration = 2f*Mathf.Pi / 5f,
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

    public void PlotRotation()
    {
        if (playing)
        {
            PlayPause(false);
        }

        var u = new MoveUnit();
        var period = 4f;
        var desRot = 3*Mathf.Pi/2f;
        var yrange = new Vector2(0f, 2 * Mathf.Pi);
        var xrange = new Vector2(0f, period);
        var init = new MovementState()
        {
            Position = new Vector2(250f, 250f),
            Rotation = 0f,
            RotVelocity = 0f,
            Velocity = new Vector2(0f, 0f)
        };
        try
        {
            var testState = MoveCommand.MakeRotation(period, Mobility, init, desRot);
            System.IO.Directory.CreateDirectory(".logs");
            using(var w = new StreamWriter(".logs/Rotation.csv"))
            {
                w.WriteLine("time|position|rotation|velocity|rotational velocity");
                testState.Preview.ToList().ForEach(x => w.WriteLine($"{x.Item1}|{x.Item2.Position}|{x.Item2.Rotation}|{x.Item2.Velocity}|{x.Item2.RotVelocity}"));
            }

            var deltaT = period/(testState.Preview.Count() - 1f);
            rangeT = xrange;
            GD.Print($"{testState.Preview.Count()} Entries, ends at Rot: {testState.Final.Rotation} Vrot: {testState.Final.RotVelocity} t: {testState.Preview.Last().Item1}");

            velPlot.SetTarget(0f, new Vector2(-Mathf.Pi, Mathf.Pi));
            velPlot.SetGrid(deltaT, Mathf.Pi/4f, xrange, yrange);
            velPlot.SetPlot("Rotating Body Velocity", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.RotVelocity)), xrange, new Vector2(-Mathf.Pi, Mathf.Pi), "Time (s)", "Rot. Velocity\n(rad/s)");
            posPlot.SetTarget(desRot, yrange);
            posPlot.SetGrid(deltaT, Mathf.Pi/4f, xrange, yrange);
            posPlot.SetPlot("Rotating Body Rotation", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.Rotation)), xrange, yrange, "Time (s)", "Rotation (rad)");

            moveAnim.SetMove(testState, deltaT, rangeT);

            SetT(rangeT[0]);
        }
        catch(Exception ex)
        {
            velPlot.Error(ex.Message);
            posPlot.Error(ex.Message);
        }
    }
}
