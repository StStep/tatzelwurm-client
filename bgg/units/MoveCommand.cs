using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Trig;

public class MoveCommand
{
    public delegate void UpdateState(MovementState state, float delta);

    private List<Tuple<float, MovementState>> _preview = new List<Tuple<float, MovementState>>();

    public const float RotationSnapDist = 0.001f;
    public const float MoveSnapDist = 0.001f;

    public float Period { get; private set; }

    public String Description { get; private set; }
    public UpdateState Update { get; private set; }
    public MovementState Initial => _preview.First().Item2;
    public MovementState Final => _preview.Last().Item2;
    public IEnumerable<Tuple<float, MovementState>> Preview => _preview;

    MoveCommand (float period, UpdateState update, MovementState initial, float previewDelta)
    {
        Period = period;
        Update = update;
        _preview.Add(new Tuple<float, MovementState>(0f, initial.Clone()));

        var temp = initial.Clone();
        for (var t = previewDelta; t <= Period; t += previewDelta)
        {
            Update(temp, previewDelta);
            _preview.Add(new Tuple<float, MovementState>(t, temp.Clone()));
        }
    }

    // TODO: Take into account current Rot Velocity for chooseing direction
    private static void RotationUpdate(IMobility mob, float rot, MovementState state, float delta)
    {
        var desireRot = Mathf.PosMod(rot, Mathf.Tau);
        var dist = Mathf.PosMod(desireRot - state.Rotation, Mathf.Tau);
        var eqdist = dist > Mathf.Pi ? dist - Mathf.Tau : dist;
        eqdist = Mathf.Abs(eqdist) < RotationSnapDist ? 0f : eqdist;

        var estRot = Mathf.PosMod(state.Rotation + state.RotVelocity * delta, 2 * Mathf.Pi);
        var estDist = Mathf.PosMod(desireRot - estRot, Mathf.Tau);
        var estEqdist = estDist > Mathf.Pi ? estDist - Mathf.Tau : estDist;
        estEqdist = Mathf.Abs(estEqdist) < RotationSnapDist ? 0f : estEqdist;

        // At target
        if (eqdist == 0f && state.RotVelocity == 0f)
        {
            // Do nothing
        }
        // Ccw
        else if (eqdist < 0f)
        {
            // If velocity will take rotation past desired rotation, and there is enough acceleration to zero out velocity, then zero them out
            if (eqdist >= state.RotVelocity * delta && state.RotVelocity >= -mob.CwAcceleration * delta)
            {
                state.RotVelocity = 0;
                state.Rotation = desireRot;
            }
            // Need to start decelerating when est rotation is within deceleration region
            else if (-estEqdist > state.RotVelocity * state.RotVelocity / (2f * mob.CwAcceleration))
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, -mob.MaxRotVelocity, delta);
            }
            else
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, Mathf.Sqrt(2f * mob.CwAcceleration * -eqdist), delta);
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
            // Need to start decelerating when est rotation is within deceleration region
            else if (estEqdist > state.RotVelocity * state.RotVelocity / (2f * mob.CcwAcceleration))
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, mob.MaxRotVelocity, delta);
            }
            else
            {
                state.RotVelocity = mob.ApproachRotVelocity(state.RotVelocity, Mathf.Sqrt(2f * mob.CcwAcceleration * eqdist), delta);
            }
        }

        state.Position += state.Velocity * delta;
        state.Rotation = Mathf.PosMod(state.Rotation + state.RotVelocity * delta, 2 * Mathf.Pi);
    }

    public static MoveCommand MakeRotation(float period, float previewDelta, IMobility mob, MovementState initial, float rot)
    {
        UpdateState up = (st, delta) =>
        {
            MoveCommand.RotationUpdate(mob, rot, st, delta);
        };
        return new MoveCommand(period, up, initial, previewDelta);
    }

    // TODO: Implement speed
    // TODO: Worry about orientation, better way of getting dir or knowing if not oriented for dir
    private static void MarchUpdate(IMobility mob, Utility.Quarter quarter, Vector2 end, float speed, MovementState state, float delta)
    {
        var eqrt = Utility.GetQuarter(state.Position, state.Rotation, end);
        var mqrt = state.Velocity == Vector2.Zero ? eqrt : Utility.GetQuarter(Vector2.Zero, state.Rotation, state.Velocity); // Vel is relative
        var dir = Utility.GetDirection(mqrt);

        var dist = (end - state.Position).Dot(dir);;
        dist = Mathf.Abs(dist) < MoveSnapDist ? 0f : dist;

        var estPos = state.Position + state.Velocity * delta;
        var estDist = (end - estPos).Dot(dir);;
        estDist = Mathf.Abs(estDist) < MoveSnapDist ? 0f : estDist;

        var dmob = mob.GetDirectionalMobility(mqrt);
        var oppmob = mob.GetDirectionalMobility(Utility.GetOppositeQuarter(mqrt));
        var deltaStop = MobilityUtility.GetDeltaStopRange(delta, dmob.Acceleration, dmob.Deceleration, oppmob.Acceleration, oppmob.Deceleration, state.Velocity.Length());
        var isWithinDelta = !float.IsNaN(deltaStop.x) && !float.IsNaN(deltaStop.y);

        if (isWithinDelta) {
            GD.Print($"{state.Position} {state.Velocity} {dist} {estDist} {deltaStop}");
        }

        // If within a delta of stopping, check min/max dist vs remaining dist
        Vector2 newV;
        if (isWithinDelta && dist >= deltaStop.x && dist <= deltaStop.y)
        {
            state.Velocity = Vector2.Zero;
            state.Position = end;
            return;
        }
        // If est position is within delta, than maintain velocity
        else if (isWithinDelta && estDist >= deltaStop.x && estDist <= deltaStop.y)
        {
            // Do nothing
            newV = state.Velocity;
        }
        // If not moving or already moving in the right direction
        else if (eqrt == mqrt)
        {
            newV = MobilityUtility.GetNextSpeed(dmob, delta, estDist < 0f ? 0f : estDist, state.Velocity.Length()) * dir;
        }
        // Else overshot, stop so it can reverse
        else {
            newV = dmob.ApproachSpeed(state.Velocity.Length(), 0f, delta) * dir;
        }

        state.Update(newV, state.RotVelocity, delta);
    }

    /// <summary>
    /// Create a straight MoveCommand based on given parameters.
    /// </summary>
    /// <param name="speed">Defaults to 0, the speed to maintain during movement; if 0, min. speed needed will be used.</param>
    public static MoveCommand MakeStraight(float period, float previewDelta, IMobility mob, MovementState initial, Utility.Quarter quarter, Vector2 end, float speed = 0)
    {
        UpdateState up = (st, delta) =>
        {
            MoveCommand.MarchUpdate(mob, quarter, end, speed, st, delta);
        };
        return new MoveCommand(period, up, initial, previewDelta);
    }

    /// <summary>
    /// Create a wheel MoveCommand based on given parameters.
    /// </summary>
    /// <param name="speed">Defaults to 0, the speed to maintain during movement; if 0, min. speed needed will be used.</param>
    public static MoveCommand MakeWheel(float period, float previewDelta, IMobility mob, MovementState initial, Arc arc, float speed = 0)
    {
        throw new NotImplementedException();
    }

    public static MoveCommand MakeWait(float period, IMobility mob, MovementState initial)
    {
        throw new NotImplementedException();
    }

    public void Log(String dir) {
        var dinfo = System.IO.Directory.CreateDirectory(dir);
        using(var w = new StreamWriter(System.IO.Path.Combine(dinfo.ToString(), "MoveCommand.csv")))
        {
            w.WriteLine("time|position|rotation|velocity|rotational velocity");
            Preview.ToList().ForEach(x => w.WriteLine($"{x.Item1}|{x.Item2.Position}|{x.Item2.Rotation}|{x.Item2.Velocity}|{x.Item2.RotVelocity}"));
        }
    }
}