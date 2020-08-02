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

    public IMobility Mobility => this.mobility.Clone();
    private Mobility mobility = new Mobility()
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

    public Boolean MarkChanges { get; private set; } = true;

    private LineEdit leCwAccel;
    private LineEdit leCcwAccel;
    private LineEdit leMaxRotVel;

    private void CwAccelChanged(String _) => SetCwAccel(Single.TryParse(leCwAccel.Text, out float v) ? v : Default.CwAcceleration);
    public void SetCwAccel(float v)
    {
        var prevText = mobility.CwAcceleration.ToString(FloatFormat);
        mobility.CwAcceleration = v;
        var newText = mobility.CwAcceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leCwAccel.Modulate = Colors.Red;
        }
        leCwAccel.Text = newText;
    }

    private void CcwAccelChanged(String _) => SetCcwAccel(Single.TryParse(leCcwAccel.Text, out float v) ? v : Default.CcwAcceleration);
    public void SetCcwAccel(float v)
    {
        var prevText = mobility.CcwAcceleration.ToString(FloatFormat);
        mobility.CcwAcceleration = v;
        var newText = mobility.CcwAcceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leCcwAccel.Modulate = Colors.Red;
        }
        leCcwAccel.Text = newText;
    }

    private void MaxRotVelChanged(String _) => SetMaxRotVel(Single.TryParse(leMaxRotVel.Text, out float v) ? v : Default.MaxRotVelocity);
    public void SetMaxRotVel(float v)
    {
        var prevText = mobility.MaxRotVelocity.ToString(FloatFormat);
        mobility.MaxRotVelocity = v;
        var newText = mobility.MaxRotVelocity.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leMaxRotVel.Modulate = Colors.Red;
        }
        leMaxRotVel.Text = newText;
    }

    private LineEdit leFrontAccel;
    private LineEdit leFrontDecel;
    private LineEdit leFrontMaxSpeed;

    private void FrontAccelChanged(String _) => SetFrontAccel(Single.TryParse(leFrontAccel.Text, out float v) ? v : Default.Front.Acceleration);
    public void SetFrontAccel(float v)
    {
        var prevText = mobility.Front.Acceleration.ToString(FloatFormat);
        mobility.Front.Acceleration = v;
        var newText = mobility.Front.Acceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leFrontAccel.Modulate = Colors.Red;
        }
        leFrontAccel.Text = newText;
    }

    private void FrontDecelChanged(String _) => SetFrontDecel(Single.TryParse(leFrontDecel.Text, out float v) ? v : Default.Front.Deceleration);
    public void SetFrontDecel(float v)
    {
        var prevText = mobility.Front.Deceleration.ToString(FloatFormat);
        mobility.Front.Deceleration = v;
        var newText = mobility.Front.Deceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leFrontDecel.Modulate = Colors.Red;
        }
        leFrontDecel.Text = newText;
    }

    private void FrontMaxSpeedChanged(String _) => SetFrontMaxSpeed(Single.TryParse(leFrontMaxSpeed.Text, out float v) ? v : Default.Front.MaxSpeed);
    public void SetFrontMaxSpeed(float v)
    {
        var prevText = mobility.Front.MaxSpeed.ToString(FloatFormat);
        mobility.Front.MaxSpeed= v;
        var newText = mobility.Front.MaxSpeed.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leFrontMaxSpeed.Modulate = Colors.Red;
        }
        leFrontMaxSpeed.Text = newText;
    }

    private LineEdit leBackAccel;
    private LineEdit leBackDecel;
    private LineEdit leBackMaxSpeed;

    private void BackAccelChanged(String _) => SetBackAccel(Single.TryParse(leBackAccel.Text, out float v) ? v : Default.Back.Acceleration);
    public void SetBackAccel(float v)
    {
        var prevText = mobility.Back.Acceleration.ToString(FloatFormat);
        mobility.Back.Acceleration = v;
        var newText = mobility.Back.Acceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leBackAccel.Modulate = Colors.Red;
        }
        leBackAccel.Text = newText;
    }

    private void BackDecelChanged(String _) => SetBackDecel(Single.TryParse(leBackDecel.Text, out float v) ? v : Default.Back.Deceleration);
    public void SetBackDecel(float v)
    {
        var prevText = mobility.Back.Deceleration.ToString(FloatFormat);
        mobility.Back.Deceleration = v;
        var newText = mobility.Back.Deceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leBackDecel.Modulate = Colors.Red;
        }
        leBackDecel.Text = newText;
    }

    private void BackMaxSpeedChanged(String _) => SetBackMaxSpeed(Single.TryParse(leBackMaxSpeed.Text, out float v) ? v : Default.Back.MaxSpeed);
    public void SetBackMaxSpeed(float v)
    {
        var prevText = mobility.Back.MaxSpeed.ToString(FloatFormat);
        mobility.Back.MaxSpeed= v;
        var newText = mobility.Back.MaxSpeed.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leBackMaxSpeed.Modulate = Colors.Red;
        }
        leBackMaxSpeed.Text = newText;
    }

    private LineEdit leLeftAccel;
    private LineEdit leLeftDecel;
    private LineEdit leLeftMaxSpeed;

    private void LeftAccelChanged(String _) => SetLeftAccel(Single.TryParse(leLeftAccel.Text, out float v) ? v : Default.Left.Acceleration);
    public void SetLeftAccel(float v)
    {
        var prevText = mobility.Left.Acceleration.ToString(FloatFormat);
        mobility.Left.Acceleration = v;
        var newText = mobility.Left.Acceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leLeftAccel.Modulate = Colors.Red;
        }
        leLeftAccel.Text = newText;
    }

    private void LeftDecelChanged(String _) => SetLeftDecel(Single.TryParse(leLeftDecel.Text, out float v) ? v : Default.Left.Deceleration);
    public void SetLeftDecel(float v)
    {
        var prevText = mobility.Left.Deceleration.ToString(FloatFormat);
        mobility.Left.Deceleration = v;
        var newText = mobility.Left.Deceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leLeftDecel.Modulate = Colors.Red;
        }
        leLeftDecel.Text = newText;
    }

    private void LeftMaxSpeedChanged(String _) => SetLeftMaxSpeed(Single.TryParse(leLeftMaxSpeed.Text, out float v) ? v : Default.Left.MaxSpeed);
    public void SetLeftMaxSpeed(float v)
    {
        var prevText = mobility.Left.MaxSpeed.ToString(FloatFormat);
        mobility.Left.MaxSpeed= v;
        var newText = mobility.Left.MaxSpeed.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leLeftMaxSpeed.Modulate = Colors.Red;
        }
        leLeftMaxSpeed.Text = newText;
    }

    private LineEdit leRightAccel;
    private LineEdit leRightDecel;
    private LineEdit leRightMaxSpeed;

    private void RightAccelChanged(String _) => SetRightAccel(Single.TryParse(leRightAccel.Text, out float v) ? v : Default.Right.Acceleration);
    public void SetRightAccel(float v)

    {
        var prevText = mobility.Right.Acceleration.ToString(FloatFormat);
        mobility.Right.Acceleration = v;
        var newText = mobility.Right.Acceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leRightAccel.Modulate = Colors.Red;
        }
        leRightAccel.Text = newText;
    }

    private void RightDecelChanged(String _) => SetRightDecel(Single.TryParse(leRightDecel.Text, out float v) ? v : Default.Right.Deceleration);
    public void SetRightDecel(float v)
    {
        var prevText = mobility.Right.Deceleration.ToString(FloatFormat);
        mobility.Right.Deceleration = v;
        var newText = mobility.Right.Deceleration.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leRightDecel.Modulate = Colors.Red;
        }
        leRightDecel.Text = newText;
    }

    private void RightMaxSpeedChanged(String _) => SetRightMaxSpeed(Single.TryParse(leRightMaxSpeed.Text, out float v) ? v : Default.Right.MaxSpeed);
    public void SetRightMaxSpeed(float v)
    {
        var prevText = mobility.Right.MaxSpeed.ToString(FloatFormat);
        mobility.Right.MaxSpeed= v;
        var newText = mobility.Right.MaxSpeed.ToString(FloatFormat);
        if (MarkChanges && prevText != newText)
        {
            leRightMaxSpeed.Modulate = Colors.Red;
        }
        leRightMaxSpeed.Text = newText;
    }

    public override void _Ready()
    {
        leCwAccel = GetNode<LineEdit>("Grid/CwAccel/LineEdit");
        leCcwAccel = GetNode<LineEdit>("Grid/CcwAccel/LineEdit");
        leMaxRotVel = GetNode<LineEdit>("Grid/MaxRotVel/LineEdit");

	    leCwAccel.Connect("text_entered", this, nameof(CwAccelChanged));
	    leCwAccel.Connect("focus_exited", this, nameof(CwAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leCcwAccel.Connect("text_entered", this, nameof(CcwAccelChanged));
	    leCcwAccel.Connect("focus_exited", this, nameof(CcwAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leMaxRotVel.Connect("text_entered", this, nameof(MaxRotVelChanged));
	    leMaxRotVel.Connect("focus_exited", this, nameof(MaxRotVelChanged), new Godot.Collections.Array() { String.Empty });

        leFrontAccel = GetNode<LineEdit>("Grid/FAccel/LineEdit");
        leFrontDecel = GetNode<LineEdit>("Grid/FDecel/LineEdit");
        leFrontMaxSpeed = GetNode<LineEdit>("Grid/FMaxSpd/LineEdit");

	    leFrontAccel.Connect("text_entered", this, nameof(FrontAccelChanged));
	    leFrontAccel.Connect("focus_exited", this, nameof(FrontAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leFrontDecel.Connect("text_entered", this, nameof(FrontDecelChanged));
	    leFrontDecel.Connect("focus_exited", this, nameof(FrontDecelChanged), new Godot.Collections.Array() { String.Empty });

	    leFrontMaxSpeed.Connect("text_entered", this, nameof(FrontMaxSpeedChanged));
	    leFrontMaxSpeed.Connect("focus_exited", this, nameof(FrontMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leBackAccel = GetNode<LineEdit>("Grid/BAccel/LineEdit");
        leBackDecel = GetNode<LineEdit>("Grid/BDecel/LineEdit");
        leBackMaxSpeed = GetNode<LineEdit>("Grid/BMaxSpd/LineEdit");

	    leBackAccel.Connect("text_entered", this, nameof(BackAccelChanged));
	    leBackAccel.Connect("focus_exited", this, nameof(BackAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leBackDecel.Connect("text_entered", this, nameof(BackDecelChanged));
	    leBackDecel.Connect("focus_exited", this, nameof(BackDecelChanged), new Godot.Collections.Array() { String.Empty });

	    leBackMaxSpeed.Connect("text_entered", this, nameof(BackMaxSpeedChanged));
	    leBackMaxSpeed.Connect("focus_exited", this, nameof(BackMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leLeftAccel = GetNode<LineEdit>("Grid/LAccel/LineEdit");
        leLeftDecel = GetNode<LineEdit>("Grid/LDecel/LineEdit");
        leLeftMaxSpeed = GetNode<LineEdit>("Grid/LMaxSpd/LineEdit");

	    leLeftAccel.Connect("text_entered", this, nameof(LeftAccelChanged));
	    leLeftAccel.Connect("focus_exited", this, nameof(LeftAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leLeftDecel.Connect("text_entered", this, nameof(LeftDecelChanged));
	    leLeftDecel.Connect("focus_exited", this, nameof(LeftDecelChanged), new Godot.Collections.Array() { String.Empty });

	    leLeftMaxSpeed.Connect("text_entered", this, nameof(LeftMaxSpeedChanged));
	    leLeftMaxSpeed.Connect("focus_exited", this, nameof(LeftMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        leRightAccel = GetNode<LineEdit>("Grid/RAccel/LineEdit");
        leRightDecel = GetNode<LineEdit>("Grid/RDecel/LineEdit");
        leRightMaxSpeed = GetNode<LineEdit>("Grid/RMaxSpd/LineEdit");

	    leRightAccel.Connect("text_entered", this, nameof(RightAccelChanged));
	    leRightAccel.Connect("focus_exited", this, nameof(RightAccelChanged), new Godot.Collections.Array() { String.Empty });

	    leRightDecel.Connect("text_entered", this, nameof(RightDecelChanged));
	    leRightDecel.Connect("focus_exited", this, nameof(RightDecelChanged), new Godot.Collections.Array() { String.Empty });

	    leRightMaxSpeed.Connect("text_entered", this, nameof(RightMaxSpeedChanged));
	    leRightMaxSpeed.Connect("focus_exited", this, nameof(RightMaxSpeedChanged), new Godot.Collections.Array() { String.Empty });

        SetDefault();
        ClearMarks();
    }

    public void Set(IMobility mob)
    {
        SetCwAccel(mob.CwAcceleration);
        SetCcwAccel(mob.CcwAcceleration);
        SetMaxRotVel(mob.MaxRotVelocity);

        SetFrontAccel(mob.Front.Acceleration);
        SetFrontDecel(mob.Front.Deceleration);
        SetFrontMaxSpeed(mob.Front.MaxSpeed);

        SetBackAccel(mob.Back.Acceleration);
        SetBackDecel(mob.Back.Deceleration);
        SetBackMaxSpeed(mob.Back.MaxSpeed);

        SetLeftAccel(mob.Left.Acceleration);
        SetLeftDecel(mob.Left.Deceleration);
        SetLeftMaxSpeed(mob.Left.MaxSpeed);

        SetRightAccel(mob.Right.Acceleration);
        SetRightDecel(mob.Right.Deceleration);
        SetRightMaxSpeed(mob.Right.MaxSpeed);
    }

    public void ClearMarks()
    {
        leCwAccel.Modulate = Colors.White;
        leCcwAccel.Modulate = Colors.White;
        leMaxRotVel.Modulate = Colors.White;

        leFrontAccel.Modulate = Colors.White;
        leFrontDecel.Modulate = Colors.White;
        leFrontMaxSpeed.Modulate = Colors.White;

        leBackAccel.Modulate = Colors.White;
        leBackDecel.Modulate = Colors.White;
        leBackMaxSpeed.Modulate = Colors.White;

        leLeftAccel.Modulate = Colors.White;
        leLeftDecel.Modulate = Colors.White;
        leLeftMaxSpeed.Modulate = Colors.White;

        leRightAccel.Modulate = Colors.White;
        leRightDecel.Modulate = Colors.White;
        leRightMaxSpeed.Modulate = Colors.White;
    }
}
