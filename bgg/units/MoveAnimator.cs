using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class MoveAnimator : Control
{
    MoveUnit moveUnit;
    MoveCommand moveCommand;
    Vector2 rangeT;

    public override void _Ready()
    {
        moveUnit = GetNode<MoveUnit>("ViewportContainer/Viewport/Unit");
    }

    public void SetMove(MoveCommand mc, Vector2 tRange)
    {
        moveCommand = mc;
        rangeT = tRange;

        moveUnit.Position = moveCommand.Initial.Position;
        moveUnit.Rotation = moveCommand.Initial.Rotation;
    }

    public void SetT(float time, float delta)
    {
        if (moveCommand == null)
            return;
        // TODO: This is a brute force way to get state at T
        var finalT = Mathf.Clamp(time, rangeT[0], rangeT[1]) - rangeT[0];
        var temp = moveCommand.Initial.Clone();
        var i = 0;
        for (var t = delta; t <= finalT; t += delta)
        {
            moveCommand.Update(temp, delta);
            i++;
        }

        moveUnit.Position = temp.Position;
        moveUnit.Rotation = temp.Rotation;
    }
}
