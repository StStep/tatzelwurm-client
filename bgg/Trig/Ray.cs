using Godot;
using System;

namespace Trig
{
    public struct Ray
    {
        public Vector2 Origin;
        public Vector2 Direction;

        public Ray(Vector2 origin, Vector2 dir)
        {
            Origin = origin;
            Direction = dir.Normalized();
        }

        public Ray(Vector2 origin, float rot)
        {
            Origin = origin;
            Direction = Vector2.Right.Rotated(rot).Normalized();
        }

        public Vector2 GetPoint(float dist) => Origin + Direction * dist;

        public Ray Tangent(Boolean clockwise = false) => new Ray(Origin, clockwise ? -Direction.Tangent() : Direction.Tangent());

        // Create a new ray with an offset relative to Vector2.Right
        public Ray RelTranslate(Vector2 offset)
        {
            var newOrigin = Origin + offset.x * Direction + offset.y * Direction.Rotated(Mathf.Pi / 2f);
            return new Ray(newOrigin, Direction);
        }

        public Ray Rotated(float phi) => new Ray(Origin, Direction.Rotated(phi));

        public float AngleToPoint(Vector2 pnt) => Direction.AngleTo(pnt - Origin);

        public Ray Snapped(Vector2 by) => new Ray(Origin.Snapped(by), Direction.Snapped(by));

        public override string ToString() => $"Ray2 at {Origin} toward {Direction}";
    }
}
