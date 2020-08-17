using Godot;
using System;

public interface IMobility
{
    float CwAcceleration { get; }
    float CcwAcceleration { get; }
    float MaxRotVelocity { get; }

    IDirectionalMobility Front { get; }
    IDirectionalMobility Back { get; }
    IDirectionalMobility Left { get; }
    IDirectionalMobility Right { get; }

    DirectionalMobility GetDirectionalMobility(Trig.Utility.Quarter quarter);
    float ApproachRotVelocity(float current, float desired, float delta);
}

public class Mobility: IMobility, ICloneable
{
    public float CwAcceleration { get; set; }
    public float CcwAcceleration { get; set; }
    public float MaxRotVelocity { get; set; }

    IDirectionalMobility IMobility.Front => Front;
    IDirectionalMobility IMobility.Back => Back;
    IDirectionalMobility IMobility.Left => Left;
    IDirectionalMobility IMobility.Right => Right;

    public DirectionalMobility Front { get; set; }
    public DirectionalMobility Back { get; set; }
    public DirectionalMobility Left { get; set; }
    public DirectionalMobility Right { get; set; }

    public DirectionalMobility GetDirectionalMobility(Trig.Utility.Quarter quarter)
    {
        switch(quarter)
        {
            case Trig.Utility.Quarter.front:
                return Front;
            case Trig.Utility.Quarter.back:
                return Back;
            case Trig.Utility.Quarter.left:
                return Left;
            default:
                return Right;
        }
    }

    public float ApproachRotVelocity(float current, float desired, float delta)
    {
        var des = Mathf.Abs(desired) <= MaxRotVelocity ? desired : (desired < 0f) ? -MaxRotVelocity : MaxRotVelocity;
        if (Mathf.IsEqualApprox(current, des))
        {
            return des;
        }
        if (current < des)
        {
            return Mathf.Abs(des - current) < CwAcceleration * delta ? des : current + CwAcceleration * delta;
        }
        else
        {
            return Mathf.Abs(des - current) < CcwAcceleration * delta ? des : current - CcwAcceleration * delta;
        }
    }

    public Mobility()
    { }

    public Mobility(Mobility other)
    {
        CwAcceleration = other.CwAcceleration;
        CcwAcceleration = other.CcwAcceleration;
        MaxRotVelocity = other.MaxRotVelocity;
        Front = other.Front.Clone();
        Back = other.Back.Clone();
        Left = other.Left.Clone();
        Right = other.Right.Clone();
    }

    public Mobility Clone() => new Mobility(this);

    object ICloneable.Clone() => Clone();
}

public interface IDirectionalMobility
{
    float Acceleration { get; }
    float Deceleration { get; }
    float MaxSpeed { get; }

    float ApproachSpeed(float current, float desired, float delta);
}

public class DirectionalMobility: IDirectionalMobility, ICloneable
{
    public float Acceleration { get; set; }
    public float Deceleration { get; set; }
    public float MaxSpeed { get; set; }

    public float ApproachSpeed(float current, float desired, float delta)
    {
        var des = desired <= MaxSpeed && desired >= 0f ? desired : (desired < 0f) ? 0f : MaxSpeed;
        if (current == des)
        {
            return des;
        }
        if (current < des)
        {
            return des - current < Acceleration * delta ? des : current + Acceleration * delta;
        }
        else
        {
            return current - des < Deceleration * delta ? des : current - Deceleration * delta;
        }
    }

    public DirectionalMobility()
    { }

    public DirectionalMobility(DirectionalMobility other)
    {
        Acceleration = other.Acceleration;
        Deceleration = other.Deceleration;
        MaxSpeed = other.MaxSpeed;
    }

    public DirectionalMobility Clone() => new DirectionalMobility(this);

    object ICloneable.Clone() => Clone();
}

// TODO: Figure out wha to do with these functions, consider moving them with approach funcs
public static class MobilityUtility
{
        // Return the max and min distance that can travel given current speed, accel, and decel
        // Max is essentially area under a triangle within a single delta where final velocity is 0, accel as much as possible but still end with 0 velocity
        // Min tries to decel as quickly as possible to zero velocity
        // Only valid if cspeed >= 0 && cspeed < decel * delta
        public static Vector2 GetDeltaStopRange(float delta, float accel, float decel, float cspeed)
        {
            if (cspeed < 0 || cspeed >= decel * delta)
                return new Vector2(float.NaN, float.NaN);

            var minDist = (cspeed * cspeed) / (2 * decel);

            var x_0_max = (decel * delta - cspeed) / (accel + decel);
            var maxDist = (cspeed * x_0_max) + (accel * x_0_max * x_0_max * 0.5f) + 0.5f * (delta - x_0_max) * (accel * x_0_max + cspeed);

            // TODO: Take into account reversing within a delta? Would need additional decel and accel for opposing dmob
            // var x_0_min = (accel * delta + cspeed) / (accel + decel);
            // var minDist = (cspeed * cspeed) / (2 * decel) - (-decel * x_0_min + cspeed) * (0.5f * (x_0_min - cspeed / decel) + 0.5f * (delta - x_0_min));

            return new Vector2(minDist, maxDist);
        }

        // Return the speed for the next tick given current speed, distance to target and accel, decel and maxspeed
        public static float GetNextSpeed(IDirectionalMobility dmob, float delta, float cdist, float cspeed)
        {
            // If decelerating now wouldn't give enough distance, increase or maintain speed
            float dspeed;
            if (cdist > Trig.Utility.AreaUnderDownRamp(cspeed, dmob.Deceleration))
            {
                dspeed = Trig.Utility.HeightOfTriWithGivenSlopesArea(cspeed, dmob.Acceleration, dmob.Deceleration, cdist);
            }
            // Else decel based on remaining dist
            else
            {
                dspeed = Trig.Utility.HeightOnDownRamp(dmob.Deceleration, cdist);
            }

            return dmob.ApproachSpeed(cspeed, dspeed, delta);
        }

}
