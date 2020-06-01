
using Godot;
using System;

namespace Trig
{
    public class Arc
    {
        public Vector2 Start { get; private set; }
        public Vector2 StartDir { get; private set; }
        public Vector2 End { get; private set; }
        public Vector2 Center { get; private set; }
        public Vector2 EndDir { get; private set; }

        // TODO Cache these values for perf
        public float Radius => Start.DistanceTo(Center);
        public float Angle => (Start - Center).AngleTo(End - Center);
        public float Length => 2f * Mathf.Pi * Radius * Mathf.Abs(Angle/(2f * Mathf.Pi));

        public Arc(Vector2 start, Vector2 startDir, Vector2 end, Vector2 endDir, Vector2 center)
        {
            Start = start;
            StartDir = startDir;
            End = end;
            EndDir = endDir;
            Center = center;
        }

        public Arc(Vector2 start, float startRot, Vector2 end)
            : this(new Ray(start, startRot), end)
        {
        }

        public Arc(Vector2 start, Vector2 startDir, Vector2 end)
            : this(new Ray(start, startDir), end)
        {
        }

        public Arc(Arc arc, float length)
            : this(arc.Start, arc.StartDir, arc.GetPoint(length))
        {
        }

        // This creates and arc by forming an isosceles triangle.
        // Start-to-end is a chord and forms the base of the triangle.
        // We know leg-A is perpendicular to the start-direction (which is always a tangent)
        // and can find the angle to the base, which will be the same angle leg-B has
        // since it's an isosceles triangle. Knowing the start and end, and then the perpendicular
        // rays to them, we can following them back to an intersection that is guaranteed to be
        // the center of the circle.
        public Arc(Ray startRay, Vector2 end)
        {
            // Invalid if pnt is in back half of dir
            if(Utility.GetQuarter(startRay, end) != Utility.Quarter.front)
                throw new ArgumentException("Outside bounds");

            Start =  startRay.Origin;
            StartDir = startRay.Direction;
            End = end;

            // Base direction is a chord, and forms base of iso triangle
            var baseDir = (End - Start).Normalized();
            bool clockwise = StartDir.AngleTo(baseDir) > 0f;

            // LegA of iso triangle is perp to StartDir (which is a tangent), and it depends on direction, points toward center
            var legADir = clockwise ? -StartDir.Tangent() : StartDir.Tangent();

            // LegB is perp to tangent at end point, and can be found because it has same corner angle as legA, points toward center
            var legBDir = (-baseDir).Rotated(legADir.AngleTo(baseDir));

            // EndDir is perp to legBDir, and it is a tangent, points toward rotation direction
            EndDir = -(clockwise ? -legBDir.Tangent() : legBDir.Tangent());

            // Intersection of extended rays is the circle center
            Center = Utility.LineIntersectionPoint(new Ray(Start, legADir), new Ray(End, legBDir));
        }

        public Vector2 GetPoint(float dist)
        {
            if (dist >= Length)
                return End;

            var angle = (dist / Length) * Angle;
            var x = Mathf.Cos(angle) * Radius;
            var y = Mathf.Sin(angle) * Radius;
            var pnt = new Vector2(x, y);

            // Rotate appropriately
            var cord = End - Start;
            var body_ang = Vector2.Down.AngleTo(StartDir);
            var pnt_ang = StartDir.AngleTo(cord);
            if (pnt_ang > 0)
                pnt = pnt.Rotated(body_ang);
            else
                pnt = pnt.Rotated(body_ang - Mathf.Pi);

            // Add center offset
            return pnt + Center;
        }

        public void Snap(Vector2 by)
        {
            Start = Start.Snapped(by);
            End = End.Snapped(by);
            Center = Center.Snapped(by);
        }
    }
}
