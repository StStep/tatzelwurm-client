
using Godot;
using System;

public class Mobility
{
    public float CwAcceleration { get; set; }
    public float CcwAcceleration { get; set; }
    public float MaxRotVelocity { get; set; }

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
}

public class DirectionalMobility
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
            return des - current < Deceleration * delta ? des : current - Deceleration * delta;
        }
    }
}
