using Godot;
using System;

public class MobilityEditor : Control
{
    public readonly String FloatFormat = "0.###";

    public void SetDefault() => Set(Default);
    public readonly IMobility Default = new Mobility()
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

    public void SetFaster() => Set(Faster);
    public readonly IMobility Faster = new Mobility()
    {
        MaxRotVelocity = Mathf.Pi / 5f,
        CwAcceleration = 9999f,
        CcwAcceleration = 9999f,
        Front = new DirectionalMobility()
        {
            MaxSpeed = 400f,
            Acceleration = 400f,
            Deceleration = 320f,
        },
        Back = new DirectionalMobility()
        {
            MaxSpeed = 200f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
        Left = new DirectionalMobility()
        {
            MaxSpeed = 200f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
        Right = new DirectionalMobility()
        {
            MaxSpeed = 200f,
            Acceleration = 200f,
            Deceleration = 160f,
        },
    };

    public IMobility Mobility => new Mobility()
    {
        MaxRotVelocity = leMaxRotVel.Value,
        CwAcceleration = leCwAccel.Value,
        CcwAcceleration = leCcwAccel.Value,
        Front = new DirectionalMobility()
        {
            MaxSpeed = leFrontMaxSpeed.Value,
            Acceleration = leFrontAccel.Value,
            Deceleration = leFrontDecel.Value,
        },
        Back = new DirectionalMobility()
        {
            MaxSpeed = leBackMaxSpeed.Value,
            Acceleration = leBackAccel.Value,
            Deceleration = leBackDecel.Value,
        },
        Left = new DirectionalMobility()
        {
            MaxSpeed = leLeftMaxSpeed.Value,
            Acceleration = leLeftAccel.Value,
            Deceleration = leLeftDecel.Value,
        },
        Right = new DirectionalMobility()
        {
            MaxSpeed = leRightMaxSpeed.Value,
            Acceleration = leRightAccel.Value,
            Deceleration = leRightDecel.Value,
        },
    };

    private LineEditWrapper<Single> leCwAccel;
    private LineEditWrapper<Single> leCcwAccel;
    private LineEditWrapper<Single> leMaxRotVel;

    private LineEditWrapper<Single> leFrontAccel;
    private LineEditWrapper<Single> leFrontDecel;
    private LineEditWrapper<Single> leFrontMaxSpeed;

    private LineEditWrapper<Single> leBackAccel;
    private LineEditWrapper<Single> leBackDecel;
    private LineEditWrapper<Single> leBackMaxSpeed;

    private LineEditWrapper<Single> leLeftAccel;
    private LineEditWrapper<Single> leLeftDecel;
    private LineEditWrapper<Single> leLeftMaxSpeed;

    private LineEditWrapper<Single> leRightAccel;
    private LineEditWrapper<Single> leRightDecel;
    private LineEditWrapper<Single> leRightMaxSpeed;

    public override void _Ready()
    {
        leCwAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/CwAccel/LineEdit"), Default.CwAcceleration, FloatFormat);
        leCcwAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/CcwAccel/LineEdit"), Default.CcwAcceleration, FloatFormat);
        leMaxRotVel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/MaxRotVel/LineEdit"), Default.MaxRotVelocity, FloatFormat);

        leCwAccel.ValueChanged = (v) => { leCwAccel.LineEdit.Modulate = Colors.Red; };
        leCcwAccel.ValueChanged = (v) => { leCcwAccel.LineEdit.Modulate = Colors.Red; };
        leMaxRotVel.ValueChanged = (v) => { leMaxRotVel.LineEdit.Modulate = Colors.Red; };

        leFrontAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/FAccel/LineEdit"), Default.Front.Acceleration, FloatFormat);
        leFrontDecel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/FDecel/LineEdit"), Default.Front.Deceleration, FloatFormat);
        leFrontMaxSpeed = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/FMaxSpd/LineEdit"), Default.Front.MaxSpeed, FloatFormat);

        leFrontAccel.ValueChanged = (v) => { leFrontAccel.LineEdit.Modulate = Colors.Red; };
        leFrontDecel.ValueChanged = (v) => { leFrontDecel.LineEdit.Modulate = Colors.Red; };
        leFrontMaxSpeed.ValueChanged = (v) => { leFrontMaxSpeed.LineEdit.Modulate = Colors.Red; };

        leBackAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/BAccel/LineEdit"), Default.Back.Acceleration, FloatFormat);
        leBackDecel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/BDecel/LineEdit"), Default.Back.Deceleration, FloatFormat);
        leBackMaxSpeed = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/BMaxSpd/LineEdit"), Default.Back.MaxSpeed, FloatFormat);

        leBackAccel.ValueChanged = (v) => { leBackAccel.LineEdit.Modulate = Colors.Red; };
        leBackDecel.ValueChanged = (v) => { leBackDecel.LineEdit.Modulate = Colors.Red; };
        leBackMaxSpeed.ValueChanged = (v) => { leBackMaxSpeed.LineEdit.Modulate = Colors.Red; };

        leLeftAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/LAccel/LineEdit"), Default.Left.Acceleration, FloatFormat);
        leLeftDecel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/LDecel/LineEdit"), Default.Left.Deceleration, FloatFormat);
        leLeftMaxSpeed = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/LMaxSpd/LineEdit"), Default.Left.MaxSpeed, FloatFormat);

        leLeftAccel.ValueChanged = (v) => { leLeftAccel.LineEdit.Modulate = Colors.Red; };
        leLeftDecel.ValueChanged = (v) => { leLeftDecel.LineEdit.Modulate = Colors.Red; };
        leLeftMaxSpeed.ValueChanged = (v) => { leLeftMaxSpeed.LineEdit.Modulate = Colors.Red; };

        leRightAccel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/RAccel/LineEdit"), Default.Right.Acceleration, FloatFormat);
        leRightDecel = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/RDecel/LineEdit"), Default.Right.Deceleration, FloatFormat);
        leRightMaxSpeed = new LineEditWrapper<Single>(GetNode<LineEdit>("Grid/RMaxSpd/LineEdit"), Default.Right.MaxSpeed, FloatFormat);

        leRightAccel.ValueChanged = (v) => { leRightAccel.LineEdit.Modulate = Colors.Red; };
        leRightDecel.ValueChanged = (v) => { leRightDecel.LineEdit.Modulate = Colors.Red; };
        leRightMaxSpeed.ValueChanged = (v) => { leRightMaxSpeed.LineEdit.Modulate = Colors.Red; };

        SetDefault();
        ClearMarks();
    }

    public void Set(IMobility mob)
    {
        leCwAccel.SetValue(mob.CwAcceleration);
        leCcwAccel.SetValue(mob.CcwAcceleration);
        leMaxRotVel.SetValue(mob.MaxRotVelocity);

        leFrontAccel.SetValue(mob.Front.Acceleration);
        leFrontDecel.SetValue(mob.Front.Deceleration);
        leFrontMaxSpeed.SetValue(mob.Front.MaxSpeed);

        leBackAccel.SetValue(mob.Back.Acceleration);
        leBackDecel.SetValue(mob.Back.Deceleration);
        leBackMaxSpeed.SetValue(mob.Back.MaxSpeed);

        leLeftAccel.SetValue(mob.Left.Acceleration);
        leLeftDecel.SetValue(mob.Left.Deceleration);
        leLeftMaxSpeed.SetValue(mob.Left.MaxSpeed);

        leRightAccel.SetValue(mob.Right.Acceleration);
        leRightDecel.SetValue(mob.Right.Deceleration);
        leRightMaxSpeed.SetValue(mob.Right.MaxSpeed);
    }

    public void ClearMarks()
    {
        leCwAccel.LineEdit.Modulate = Colors.White;
        leCcwAccel.LineEdit.Modulate = Colors.White;
        leMaxRotVel.LineEdit.Modulate = Colors.White;

        leFrontAccel.LineEdit.Modulate = Colors.White;
        leFrontDecel.LineEdit.Modulate = Colors.White;
        leFrontMaxSpeed.LineEdit.Modulate = Colors.White;

        leBackAccel.LineEdit.Modulate = Colors.White;
        leBackDecel.LineEdit.Modulate = Colors.White;
        leBackMaxSpeed.LineEdit.Modulate = Colors.White;

        leLeftAccel.LineEdit.Modulate = Colors.White;
        leLeftDecel.LineEdit.Modulate = Colors.White;
        leLeftMaxSpeed.LineEdit.Modulate = Colors.White;

        leRightAccel.LineEdit.Modulate = Colors.White;
        leRightDecel.LineEdit.Modulate = Colors.White;
        leRightMaxSpeed.LineEdit.Modulate = Colors.White;
    }
}
