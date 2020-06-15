using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Trig;

public class MoveCommand
{
    public delegate void UpdateState(MovementState state, float delta);

    private List<Tuple<float, MovementState>> _preview = new List<Tuple<float, MovementState>>();

    public float Period { get; private set; }

    public UpdateState Update { get; private set; }
    public MovementState Initial => _preview.First().Item2;
    public MovementState Final => _preview.Last().Item2;
    public IEnumerable<Tuple<float, MovementState>> Preview => _preview;

    MoveCommand (float period, UpdateState update, MovementState initial)
    {
        Period = period;
        Update = update;
        _preview.Add(new Tuple<float, MovementState>(0f, initial.Clone()));

        var temp = initial;
        var delta = Period/100f;
        for (var t = delta; t <= Period; t += delta)
        {
            Update(temp, delta);
            _preview.Add(new Tuple<float, MovementState>(t, temp.Clone()));
        }
    }

    // TODO: Take into account current Rot Velocity for choseing direction
    private static void RotationUpdate(Mobility mob, float rot, MovementState state, float delta)
    {
        var desireRot = Mathf.PosMod(rot, Mathf.Tau);
        var dist = Mathf.PosMod(desireRot - state.Rotation, Mathf.Tau);
        var eqdist = dist > Mathf.Pi ? dist - Mathf.Tau : dist;
        var estRot = Mathf.PosMod(state.Rotation + state.RotVelocity * delta, 2 * Mathf.Pi);

        if (Mathf.IsEqualApprox(state.Rotation, desireRot))
        {
            state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, 0f, delta);
        }
        // Ccw
        else if (eqdist < 0f)
        {
            // If velocity will take rotation past desired rotation, and there is enough acceleration to zero out velocity, then zero them out
            if (eqdist >= state.RotVelocity*delta && state.RotVelocity >= -mob.CwAcceleration * delta)
            {
                state.RotVelocity = 0;
                state.Rotation = desireRot;
            }
            // Outside deceleration region
            else if (-eqdist > state.RotVelocity * state.RotVelocity / (2f * mob.CwAcceleration))
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, -mob.MaxRotVelocity, delta);
            }
            else
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, 0f, delta);
            }
        }
        // Cw
        else
        {
            // If velocity will take rotation past desired rotation, and there is enough acceleration to zero out velocity, then zero them out
            if (eqdist <= state.RotVelocity*delta && state.RotVelocity <= mob.CcwAcceleration * delta)
            {
                state.RotVelocity = 0;
                state.Rotation = desireRot;
            }
            // Outside deceleration region
            else if (eqdist > state.RotVelocity * state.RotVelocity / (2f * mob.CcwAcceleration))
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, mob.MaxRotVelocity, delta);
            }
            else
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, 0f, delta);
            }
        }

        state.Position += state.Velocity * delta;
        state.Rotation = Mathf.PosMod(state.Rotation + state.RotVelocity * delta, 2 * Mathf.Pi);
    }

    public static MoveCommand MakeRotation(float period, Mobility mob, MovementState initial, float rot)
    {
        UpdateState up = (st, delta) =>
        {
            MoveCommand.RotationUpdate(mob, rot, st, delta);
        };
        return new MoveCommand(period, up, initial);
    }

    /// <summary>
    /// Create a straight MoveCommand based on given parameters.
    /// </summary>
    /// <param name="speed">Defaults to 0, the speed to maintain during movement; if 0, min. speed needed will be used.</param>
    public static MoveCommand MakeStraight(float period, Mobility mob, MovementState initial, Utility.Quarter quarter, Vector2 end, float speed = 0)
    {
        var endRot = (end - initial.Position).AngleTo(Vector2.Right);
        var sideRot = Utility.GetRotation(quarter);
        var desireRot = Mathf.PosMod(endRot - sideRot, 2 * Mathf.Pi);
        UpdateState up = (st, delta) =>
        {
            // First zero any rotational vel
            if (!Mathf.IsEqualApprox(st.Rotation, desireRot))
            {
                MoveCommand.RotationUpdate(mob, desireRot, st, delta);
            }
            else
            {
                // TODO: Handle movement
                throw new NotImplementedException();
                st.Position += st.Velocity * delta;
                st.Rotation = Mathf.PosMod(st.Rotation + st.RotVelocity * delta, 2 * Mathf.Pi);
            }

        };
        return new MoveCommand(period, up, initial);
    }

    public static MoveCommand MakeWheel(float period, Mobility mob, MovementState initial, Arc arc)
    {
        throw new NotImplementedException();
    }

    public static MoveCommand MakeWait(float period, Mobility mob, MovementState initial)
    {
        throw new NotImplementedException();
    }
}