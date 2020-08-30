using System;
using Godot;

public class MovementState: ICloneable
{
    // ! Make private?
    public Vector2 Velocity { get; set; }
    public float RotVelocity { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    public MovementState()
    { }

    public MovementState(MovementState other)
    {
        Velocity = other.Velocity;
        RotVelocity = other.RotVelocity;
        Position = other.Position;
        Rotation = other.Rotation;
    }

    public MovementState Clone() => new MovementState(this);

    object ICloneable.Clone() => Clone();

    public void Update(Vector2 vel, float rvel, float delta) {
        var pvel = Velocity;
        Velocity = vel;
        var prvel = RotVelocity;
        RotVelocity = rvel;

        Position = new Vector2(Position.x + Trig.Utility.AreaUnderRightTrapezoid(pvel.x, Velocity.x, delta),
                               Position.y + Trig.Utility.AreaUnderRightTrapezoid(pvel.y, Velocity.y, delta));
        Rotation = Mathf.PosMod(Rotation + Trig.Utility.AreaUnderRightTrapezoid(prvel, RotVelocity, delta), 2 * Mathf.Pi);
    }
}
