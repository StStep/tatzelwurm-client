using System;
using Godot;

public class MovementState: ICloneable
{
    public Vector2 Velocity { get; set; }
    public float RotVelocity { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; }

    public MovementState(MovementState other)
    {
        Velocity = other.Velocity;
        RotVelocity = other.RotVelocity;
        Position = other.Position;
        Rotation = other.Rotation;
    }

    public MovementState Clone() => new MovementState(this);

    object ICloneable.Clone()
    {
        return Clone();
    }
}