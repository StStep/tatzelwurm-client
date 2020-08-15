using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Trig;

public class MoveCommandTB : Control
{
    Plot posPlot;
    Plot velPlot;
    MoveAnimator moveAnim;
    CheckButton playToggle;
    Label tLabel;
    Slider tSlider;
    MobilityEditor mobEditor;
    LineEditWrapper<Single> lePeriod;
    LineEditWrapper<Single> leDelta;

    // Rotation Controls
    LineEditWrapper<Single> leDesiredRot;

    // March Controls
    LineEditWrapper<Single> leDesiredSpeed;
    LineEditWrapper<Single> leDesiredDist;

    Vector2 rangeT;
    float curT;
    Boolean playing;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        posPlot = GetNode<Plot>("PositionPlot");
        velPlot = GetNode<Plot>("VelocityPlot");
        moveAnim = GetNode<MoveAnimator>("MoveAnimator");
        playToggle = GetNode<CheckButton>("PlaybackBox/PlayToggle");
        tLabel = GetNode<Label>("PlaybackBox/CurrentT");
        tSlider = GetNode<Slider>("PlaybackBox/TimeSlider");
        mobEditor = GetNode<MobilityEditor>("MobilityEditor");

        lePeriod = new LineEditWrapper<Single>(GetNode<LineEdit>("MiscFields/lePeriod"), 4f, "0.00");
        lePeriod.ValueChanged = (v) => { lePeriod.LineEdit.Modulate = Colors.Red; };

        leDelta = new LineEditWrapper<Single>(GetNode<LineEdit>("MiscFields/leDelta"), 0.04f, "0.00");
        leDelta.ValueChanged = (v) => { leDelta.LineEdit.Modulate = Colors.Red; };

        // Hook up Desired Rotation and restrict to radians
        leDesiredRot = new LineEditWrapper<Single>(GetNode<LineEdit>("MoveTabs/Rotation/Parameters/leDrot"), 3*Mathf.Pi/2f, "0.###");
        leDesiredRot.ValueChanged = (v) => { if (v < 0f || v > Mathf.Tau) leDesiredRot.SetValue(Mathf.Wrap(v, 0f, Mathf.Tau)); };
        leDesiredRot.ValueChanged += (v) => { leDesiredRot.LineEdit.Modulate = Colors.Red; };

        leDesiredSpeed = new LineEditWrapper<Single>(GetNode<LineEdit>("MoveTabs/March/Parameters/leDspd"), 0, "0.###");
        leDesiredDist = new LineEditWrapper<Single>(GetNode<LineEdit>("MoveTabs/March/Parameters/leDdist"), 350f, "0");

        playToggle.Connect("toggled", this, nameof(PlayPause));

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
        if (playToggle.Pressed != playing)
            playToggle.Pressed = playing;
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

    public void PlotRotation()
    {
        if (playing)
        {
            PlayPause(false);
        }

        var u = new MoveUnit();
        var yrange = new Vector2(0f, 2 * Mathf.Pi);
        var xrange = new Vector2(0f, lePeriod.Value);
        var init = new MovementState()
        {
            Position = new Vector2(250f, 250f),
            Rotation = 0f,
            RotVelocity = 0f,
            Velocity = new Vector2(0f, 0f)
        };
        try
        {
            var testState = MoveCommand.MakeRotation(lePeriod.Value, leDelta.Value, mobEditor.Mobility, init, leDesiredRot.Value);
            testState.Log(".logs");
            mobEditor.ClearMarks();

            rangeT = xrange;
            GD.Print($"{testState.Preview.Count()} Entries, ends at Rot: {testState.Final.Rotation} Vrot: {testState.Final.RotVelocity} t: {testState.Preview.Last().Item1}");

            posPlot.SetTarget(leDesiredRot.Value, yrange);
            posPlot.SetGrid(leDelta.Value, Mathf.Pi/4f, xrange, yrange);
            posPlot.SetPlot("Rotating Body Rotation", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.Rotation)), xrange, yrange, "Time (s)", "Rotation (rad)");
            velPlot.SetTarget(0f, new Vector2(-Mathf.Pi, Mathf.Pi));
            velPlot.SetGrid(leDelta.Value, Mathf.Pi/4f, xrange, yrange);
            velPlot.SetPlot("Rotating Body Velocity", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.RotVelocity)), xrange, new Vector2(-Mathf.Pi, Mathf.Pi), "Time (s)", "Rot. Velocity\n(rad/s)");

            moveAnim.SetMove(testState, leDelta.Value, rangeT);

            SetT(rangeT[0]);

            lePeriod.LineEdit.Modulate = Colors.White;
            leDelta.LineEdit.Modulate = Colors.White;
            leDesiredRot.LineEdit.Modulate = Colors.White;
        }
        catch(Exception ex)
        {
            velPlot.Error(ex.Message);
            posPlot.Error(ex.Message);
        }
    }

    public void PlotMarch()
    {
        if (playing)
        {
            PlayPause(false);
        }

        var u = new MoveUnit();
        var init = new MovementState()
        {
            Position = new Vector2(250f, 250f),
            Rotation = 0f,
            RotVelocity = 0f,
            Velocity = new Vector2(0f, 0f)
        };
        var mvQuarter = Utility.Quarter.front;
        var mvEnd = init.Position + new Vector2(leDesiredDist.Value, 0);
        try
        {
            var testState = MoveCommand.MakeStraight(lePeriod.Value, leDelta.Value, mobEditor.Mobility, init, mvQuarter, mvEnd, leDesiredSpeed.Value);
            testState.Log(".logs");
            mobEditor.ClearMarks();

            rangeT = new Vector2(0f, lePeriod.Value);
            var velrange = new Vector2(-mobEditor.Mobility.Back.MaxSpeed * 1.1f, mobEditor.Mobility.Front.MaxSpeed * 1.1f);
            var distrange = new Vector2(-500f, 100f);

            posPlot.SetTarget(0f, distrange);
            posPlot.SetGrid(leDelta.Value, 50f, rangeT, distrange);
            posPlot.SetPlot("Distance to Target", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.Position.x - mvEnd.x)), rangeT, distrange, "Time (s)", "Horiz. Dist. (px)");
            velPlot.SetTarget(0f, velrange);
            velPlot.SetGrid(leDelta.Value, 25f, rangeT, velrange);
            velPlot.SetPlot("Horizontal Speed", testState.Preview.Select(p => new Vector2(p.Item1, p.Item2.Velocity.Dot(Vector2.Right))), rangeT, velrange, "Time (s)", "Horiz. Speed\n(px/s)");

            moveAnim.SetMove(testState, leDelta.Value, rangeT);

            SetT(rangeT[0]);

            lePeriod.LineEdit.Modulate = Colors.White;
            leDelta.LineEdit.Modulate = Colors.White;
        }
        catch(Exception ex)
        {
            velPlot.Error(ex.Message);
            posPlot.Error(ex.Message);
        }
    }
}
