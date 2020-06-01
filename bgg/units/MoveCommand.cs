using Godot;
using System;
using System.Collections.Generic;
using Trig;

public class MoveCommand
{
    public float Period { get; private set; } = 25f;

    public Vector2 Start { get; private set; }
    public Func<float, Vector2> VelocityFunc { get; private set; }
    public Func<float, float> HeadingFunc { get; private set; }

    public Vector2[] Preview(int samples)
    {
        var step = Period/(samples - 1);
        var pos = new Vector2[samples];
        pos[0] = Start;
        for(int i = 1; i < samples; i++)
        {
            // var b = step * i;
            // var a = step * (i - 1);
            // var integral = (b - a) * (VelocityFunc.Invoke(a) + VelocityFunc.Invoke(b)) / 2;
            pos[i] = pos[i - 1] + step * VelocityFunc.Invoke(step * (i - 1));
        }

        return pos;
    }

    public static MoveCommand MakeRotation(Vector2 gpos, float startRot, float endRot)
    {
        var ret = new MoveCommand();
        ret.Start = gpos;
        ret.VelocityFunc = t => Vector2.Zero;
        ret.HeadingFunc = t => Mathf.LerpAngle(startRot, endRot, t/ret.Period);
        return ret;
    }

    public static MoveCommand MakeStraight(Vector2 start, Vector2 end, float rot)
    {
        var ret = new MoveCommand();
        ret.Start = start;
        ret.VelocityFunc = t => (end - start)/ret.Period;
        ret.HeadingFunc = t => rot;
        return ret;
    }

    public static MoveCommand MakeWheel(Arc arc)
    {
        var ret = new MoveCommand();
        ret.Start = arc.Start;
        ret.VelocityFunc = t => Vector2.Right.Rotated(ret.HeadingFunc.Invoke(t)) * (arc.Length/ret.Period);
        ret.HeadingFunc = t =>  Mathf.LerpAngle(arc.StartDir.Angle(), arc.EndDir.Angle(), t/ret.Period);
        return ret;
    }
}