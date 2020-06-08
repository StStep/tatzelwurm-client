using Godot;
using System;
using System.Collections.Generic;
using Trig;

public class MoveCommand
{
    public float Period { get; private set; } = 2.5f;

    public Vector2 Start { get; private set; }
    public Func<float, Vector2> VelocityFunc { get; private set; }
    public Func<float, float> HeadingFunc { get; private set; }

    public float StartHeading => HeadingFunc?.Invoke(0) ?? 0f;
    public float EndHeading => HeadingFunc?.Invoke(Period) ?? 0f;
    public Vector2 StartVelocity => VelocityFunc?.Invoke(0) ?? Vector2.Zero;
    public Vector2 EndVelocity => VelocityFunc?.Invoke(Period) ?? Vector2.Zero;

    public Ray[] Preview(int samples)
    {
        var step = Period/(samples - 1);
        var ray = new Ray[samples];
        ray[0] = new Ray(Start, HeadingFunc.Invoke(0));
        for(int i = 1; i < samples; i++)
        {
            ray[i] = new Ray(ray[i - 1].Origin + step * VelocityFunc.Invoke(step * (i - 1)), HeadingFunc.Invoke(step * (i -1)));
        }

        return ray;
    }

    public Vector2[] PreviewPath(int samples)
    {
        var step = Period/(samples - 1);
        var pos = new Vector2[samples];
        pos[0] = Start;
        for(int i = 1; i < samples; i++)
        {
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

    public static MoveCommand MakeWait(Vector2 start, float rot)
    {
        var ret = new MoveCommand();
        ret.Start = start;
        ret.VelocityFunc = t => Vector2.Zero;
        ret.HeadingFunc = t => rot;
        return ret;
    }
}