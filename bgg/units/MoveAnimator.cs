using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class MoveAnimator : Control
{
    MoveUnit moveUnit;
    MoveCommand moveCommand;
    float deltaT;
    Vector2 rangeT;

    public override void _Ready()
    {
        moveUnit = GetNode<MoveUnit>("ViewportContainer/Viewport/Unit");
    }

    public void SetMove(MoveCommand mc, float delta, Vector2 tRange)
    {
        moveCommand = mc;
        deltaT = delta;
        rangeT = tRange;

        moveUnit.Position = moveCommand.Initial.Position;
        moveUnit.Rotation = moveCommand.Initial.Rotation;
    }

    public void SetT(float time)
    {
        if (moveCommand == null)
            return;
        // TODO: This is a brute force way to get state at T
        var finalT = Mathf.Clamp(time, rangeT[0], rangeT[1]) - rangeT[0];
        var temp = moveCommand.Initial.Clone();
        var i = 0;
        for (var t = deltaT; t <= finalT; t += deltaT)
        {
            moveCommand.Update(temp, deltaT);
            i++;
        }

        moveUnit.Position = temp.Position;
        moveUnit.Rotation = temp.Rotation;
    }
}
