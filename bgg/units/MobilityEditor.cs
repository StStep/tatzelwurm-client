using Godot;
using System;

public class MobilityEditor : GridContainer
{
    public readonly String FloatFormat = "0.###";
    public readonly Mobility Default = new Mobility()
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

    public Mobility Mobility { get; private set; } = new Mobility()
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

    private LineEdit leCwAccel;
    private LineEdit leCcwAccel;
    private LineEdit leMaxRotVel;

    private void CwAccelChanged(String _) => SetCwAccel(Single.TryParse(leCwAccel.Text, out float v) ? v : Default.CwAcceleration);
    public void SetCwAccel(float v)
    {
        Mobility.CwAcceleration = v;
        leCwAccel.Text = Mobility.CwAcceleration.ToString(FloatFormat);
    }

    private void CcwAccelChanged(String _) => SetCcwAccel(Single.TryParse(leCcwAccel.Text, out float v) ? v : Default.CcwAcceleration);
    public void SetCcwAccel(float v)
    {
        Mobility.CcwAcceleration = v;
        leCcwAccel.Text = Mobility.CcwAcceleration.ToString(FloatFormat);
    }

    private void MaxRotVelChanged(String _) => SetMaxRotVel(Single.TryParse(leMaxRotVel.Text, out float v) ? v : Default.MaxRotVelocity);
    public void SetMaxRotVel(float v)
    {
        Mobility.MaxRotVelocity = v;
        leMaxRotVel.Text = Mobility.MaxRotVelocity.ToString(FloatFormat);
    }

    private LineEdit leFrontAccel;
    private LineEdit leFrontDecel;
    private LineEdit leFrontMaxSpeed;

    private void FrontAccelChanged(String _) => SetFrontAccel(Single.TryParse(leFrontAccel.Text, out float v) ? v : Default.Front.Acceleration);
    public void SetFrontAccel(float v)
    {
        Mobility.Front.Acceleration = v;
        leFrontAccel.Text = Mobility.Front.Acceleration.ToString(FloatFormat);
    }

    private void FrontDecelChanged(String _) => SetFrontDecel(Single.TryParse(leFrontDecel.Text, out float v) ? v : Default.Front.Deceleration);
    public void SetFrontDecel(float v)
    {
        Mobility.Front.Deceleration = v;
        leFrontDecel.Text = Mobility.Front.Deceleration.ToString(FloatFormat);
    }

    private void FrontMaxSpeedChanged(String _) => SetFrontMaxSpeed(Single.TryParse(leFrontMaxSpeed.Text, out float v) ? v : Default.Front.MaxSpeed);
    public void SetFrontMaxSpeed(float v)
    {
        Mobility.Front.MaxSpeed= v;
        leFrontMaxSpeed.Text = Mobility.Front.MaxSpeed.ToString(FloatFormat);
    }

    private LineEdit leBackAccel;
    private LineEdit leBackDecel;
    private LineEdit leBackMaxSpeed;

    private void BackAccelChanged(String _) => SetBackAccel(Single.TryParse(leBackAccel.Text, out float v) ? v : Default.Back.Acceleration);
    public void SetBackAccel(float v)
    {
        Mobility.Back.Acceleration = v;
        leBackAccel.Text = Mobility.Back.Acceleration.ToString(FloatFormat);
    }

    private void BackDecelChanged(String _) => SetBackDecel(Single.TryParse(leBackDecel.Text, out float v) ? v : Default.Back.Deceleration);
    public void SetBackDecel(float v)
    {
        Mobility.Back.Deceleration = v;
        leBackDecel.Text = Mobility.Back.Deceleration.ToString(FloatFormat);
    }

    private void BackMaxSpeedChanged(String _) => SetBackMaxSpeed(Single.TryParse(leBackMaxSpeed.Text, out float v) ? v : Default.Back.MaxSpeed);
    public void SetBackMaxSpeed(float v)
    {
        Mobility.Back.MaxSpeed= v;
        leBackMaxSpeed.Text = Mobility.Back.MaxSpeed.ToString(FloatFormat);
    }

    private LineEdit leLeftAccel;
    private LineEdit leLeftDecel;
    private LineEdit leLeftMaxSpeed;

    private void LeftAccelChanged(String _) => SetLeftAccel(Single.TryParse(leLeftAccel.Text, out float v) ? v : Default.Left.Acceleration);
    public void SetLeftAccel(float v)
    {
        Mobility.Left.Acceleration = v;
        leLeftAccel.Text = Mobility.Left.Acceleration.ToString(FloatFormat);
    }

    private void LeftDecelChanged(String _) => SetLeftDecel(Single.TryParse(leLeftDecel.Text, out float v) ? v : Default.Left.Deceleration);
    public void SetLeftDecel(float v)
    {
        Mobility.Left.Deceleration = v;
        leLeftDecel.Text = Mobility.Left.Deceleration.ToString(FloatFormat);
    }

    private void LeftMaxSpeedChanged(String _) => SetLeftMaxSpeed(Single.TryParse(leLeftMaxSpeed.Text, out float v) ? v : Default.Left.MaxSpeed);
    public void SetLeftMaxSpeed(float v)
    {
        Mobility.Left.MaxSpeed= v;
        leLeftMaxSpeed.Text = Mobility.Left.MaxSpeed.ToString(FloatFormat);
    }

    private LineEdit leRightAccel;
    private LineEdit leRightDecel;
    private LineEdit leRightMaxSpeed;

    private void RightAccelChanged(String _) => SetRightAccel(Single.TryParse(leRightAccel.Text, out float v) ? v : Default.Right.Acceleration);
    public void SetRightAccel(float v)
    {
        Mobility.Right.Acceleration = v;
        leRightAccel.Text = Mobility.Right.Acceleration.ToString(FloatFormat);
    }

    private void RightDecelChanged(String _) => SetRightDecel(Single.TryParse(leRightDecel.Text, out float v) ? v : Default.Right.Deceleration);
    public void SetRightDecel(float v)
    {
        Mobility.Right.Deceleration = v;
        leRightDecel.Text = Mobility.Right.Deceleration.ToString(FloatFormat);
    }

    private void RightMaxSpeedChanged(String _) => SetRightMaxSpeed(Single.TryParse(leRightMaxSpeed.Text, out float v) ? v : Default.Right.MaxSpeed);
    public void SetRightMaxSpeed(float v)
    {
        Mobility.Right.MaxSpeed= v;
        leRightMaxSpeed.Text = Mobility.Right.MaxSpeed.ToString(FloatFormat);
    }

    public override void _Ready()
    {
        leCwAccel = GetNode<LineEdit>("CwAccel/LineEdit");
        leCcwAccel = GetNode<LineEdit>("CcwAccel/LineEdit");
        leMaxRotVel = GetNode<LineEdit>("MaxRotVel/LineEdit");

        SetCwAccel(Mobility.CwAcceleration);
	    leCwAccel.Connect("text_entered", this, nameof(CwAccelChanged));
	    leCwAccel.Connect("focus_exited", this, nameof(CwAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetCcwAccel(Mobility.CcwAcceleration);
	    leCcwAccel.Connect("text_entered", this, nameof(CcwAccelChanged));
	    leCcwAccel.Connect("focus_exited", this, nameof(CcwAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetMaxRotVel(Mobility.MaxRotVelocity);
	    leMaxRotVel.Connect("text_entered", this, nameof(MaxRotVelChanged));
	    leMaxRotVel.Connect("focus_exited", this, nameof(MaxRotVelChanged), new Godot.Collections.Array() { String.Empty });

        leFrontAccel = GetNode<LineEdit>("FAccel/LineEdit");
        leFrontDecel = GetNode<LineEdit>("FDecel/LineEdit");
        leFrontMaxSpeed = GetNode<LineEdit>("FMaxSpd/LineEdit");

        SetFrontAccel(Mobility.Front.Acceleration);
	    leFrontAccel.Connect("text_entered", this, nameof(FrontAccelChanged));
	    leFrontAccel.Connect("focus_exited", this, nameof(FrontAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetFrontDecel(Mobility.Front.Deceleration);
	    leFrontDecel.Connect("text_entered", this, nameof(FrontDecelChanged));
	    leFrontDecel.Connect("focus_exited", this, nameof(FrontDecelChanged), new Godot.Collections.Array() { String.Empty });

        SetFrontMaxSpeed(Mobility.Front.MaxSpeed);
	    leFrontMaxSpeed.Connect("text_entered", this, nameof(FrontMaxSpeedChanged));
	    leFrontMaxSpeed.Connect("focus_exited", this, nameof(FrontMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leBackAccel = GetNode<LineEdit>("BAccel/LineEdit");
        leBackDecel = GetNode<LineEdit>("BDecel/LineEdit");
        leBackMaxSpeed = GetNode<LineEdit>("BMaxSpd/LineEdit");

        SetBackAccel(Mobility.Back.Acceleration);
	    leBackAccel.Connect("text_entered", this, nameof(BackAccelChanged));
	    leBackAccel.Connect("focus_exited", this, nameof(BackAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetBackDecel(Mobility.Back.Deceleration);
	    leBackDecel.Connect("text_entered", this, nameof(BackDecelChanged));
	    leBackDecel.Connect("focus_exited", this, nameof(BackDecelChanged), new Godot.Collections.Array() { String.Empty });

        SetBackMaxSpeed(Mobility.Back.MaxSpeed);
	    leBackMaxSpeed.Connect("text_entered", this, nameof(BackMaxSpeedChanged));
	    leBackMaxSpeed.Connect("focus_exited", this, nameof(BackMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leLeftAccel = GetNode<LineEdit>("LAccel/LineEdit");
        leLeftDecel = GetNode<LineEdit>("LDecel/LineEdit");
        leLeftMaxSpeed = GetNode<LineEdit>("LMaxSpd/LineEdit");

        SetLeftAccel(Mobility.Left.Acceleration);
	    leLeftAccel.Connect("text_entered", this, nameof(LeftAccelChanged));
	    leLeftAccel.Connect("focus_exited", this, nameof(LeftAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetLeftDecel(Mobility.Left.Deceleration);
	    leLeftDecel.Connect("text_entered", this, nameof(LeftDecelChanged));
	    leLeftDecel.Connect("focus_exited", this, nameof(LeftDecelChanged), new Godot.Collections.Array() { String.Empty });

        SetLeftMaxSpeed(Mobility.Left.MaxSpeed);
	    leLeftMaxSpeed.Connect("text_entered", this, nameof(LeftMaxSpeedChanged));
	    leLeftMaxSpeed.Connect("focus_exited", this, nameof(LeftMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leRightAccel = GetNode<LineEdit>("RAccel/LineEdit");
        leRightDecel = GetNode<LineEdit>("RDecel/LineEdit");
        leRightMaxSpeed = GetNode<LineEdit>("RMaxSpd/LineEdit");

        SetRightAccel(Mobility.Right.Acceleration);
	    leRightAccel.Connect("text_entered", this, nameof(RightAccelChanged));
	    leRightAccel.Connect("focus_exited", this, nameof(RightAccelChanged), new Godot.Collections.Array() { String.Empty });

        SetRightDecel(Mobility.Right.Deceleration);
	    leRightDecel.Connect("text_entered", this, nameof(RightDecelChanged));
	    leRightDecel.Connect("focus_exited", this, nameof(RightDecelChanged), new Godot.Collections.Array() { String.Empty });

        SetRightMaxSpeed(Mobility.Right.MaxSpeed);
	    leRightMaxSpeed.Connect("text_entered", this, nameof(RightMaxSpeedChanged));
	    leRightMaxSpeed.Connect("focus_exited", this, nameof(RightMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });
    }

}
