using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// </summary>
public static class Trig
{
    public struct Arc2
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

        public Arc2(Vector2 start, Vector2 startDir, Vector2 end, Vector2 endDir, Vector2 center)
        {
            Start = start;
            StartDir = startDir;
            End = end;
            EndDir = endDir;
            Center = center;
        }

        public Arc2(Vector2 start, float startRot, Vector2 end)
            : this(new Ray2(start, startRot), end)
        {
        }

        public Arc2(Vector2 start, Vector2 startDir, Vector2 end)
            : this(new Ray2(start, startDir), end)
        {
        }

        // This creates and arc by forming an isosceles triangle.
        // Start-to-end is a chord and forms the base of the triangle.
        // We know leg-A is perpendicular to the start-direction (which is always a tangent)
        // and can find the angle to the base, which will be the same angle leg-B has
        // since it's an isosceles triangle. Knowing the start and end, and then the perpendicular
        // rays to them, we can following them back to an intercection that is guaranteed to be
        // the center of the circle.
        public Arc2(Ray2 startRay, Vector2 end)
        {
            // Invalid if pnt is in back half of dir
            if(GetQuarter(startRay, end) != Quarter.front)
                throw new ArgumentException("Outside bounds");

            Start =  startRay.Origin;
            StartDir = startRay.Direction;
            End = end;

            // Base direction is a chord, and froms base of iso triangle
            var baseDir = (End - Start).Normalized();
            bool clockwise = StartDir.AngleTo(baseDir) > 0f;

            // LegA of iso triangle is perp to StartDir (which is a tangent), and it depends on direction, points toward center
            var legADir = clockwise ? -StartDir.Tangent() : StartDir.Tangent();

            // LegB is perp to tangent at end point, and can be found because it has same corner angle as legA, points toward center
            var legBDir = (-baseDir).Rotated(legADir.AngleTo(baseDir));

            // EndDir is perp to legBDir, and it is a tangent, points toward rotation direction
            EndDir = -(clockwise ? -legBDir.Tangent() : legBDir.Tangent());

            // Intesection of extended rays is the circle center
            Center = LineIntersectionPoint(new Ray2(Start, legADir), new Ray2(End, legBDir));
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
            StartDir = StartDir.Snapped(by);
            End = End.Snapped(by);
            EndDir = EndDir.Snapped(by);
            Center = Center.Snapped(by);
        }
    }

    public struct Ray2
    {
        public Vector2 Origin;
        public Vector2 Direction;

        public Ray2(Vector2 origin, Vector2 dir)
        {
            Origin = origin;
            Direction = dir.Normalized();
        }

        public Ray2(Vector2 origin, float rot)
        {
            Origin = origin;
            Direction = Vector2.Right.Rotated(rot).Normalized();
        }

        public Vector2 GetPoint(float dist) => Origin + Direction * dist;

        public Ray2 Tangent(Boolean clockwise = false) => new Ray2(Origin, clockwise ? -Direction.Tangent() : Direction.Tangent());

        // Create a new ray with an offset relative to Vector2.Right
        public Ray2 RelTranslate(Vector2 offset)
        {
            var newOrigin = Origin + offset.x * Direction + offset.y * Direction.Rotated(Mathf.Pi / 2f);
            return new Ray2(newOrigin, Direction);
        }

        public Ray2 Snapped(Vector2 by) => new Ray2(Origin.Snapped(by), Direction.Snapped(by));

        public override string ToString() => $"Ray2 at {Origin} toward {Direction}";
    }

    public static float DistToLine(Ray2 dir, Vector2 pnt)
    {
        Vector2 linepnt = NearestPointOnLine(dir, pnt);
        return linepnt.DistanceTo(pnt);
    }

    public static Vector2 NearestPointOnLine(Ray2 dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.Origin;
        float d = v.Dot(dir.Direction);
        return dir.GetPoint(d);
    }

    // Imagine a line through origin cutting a plane in half, perpendicular to direction
    public enum PerpHalf { front, back };
    public static PerpHalf GetPerpHalf(Vector2 Origin, Vector2 direction, Vector2 pnt)
    {
        return GetPerpHalf(new Ray2(Origin, direction), pnt);
    }
    public static PerpHalf GetPerpHalf(Ray2 dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.Origin;
        return (Mathf.Abs(v.AngleTo(dir.Direction)) <= Mathf.Pi / 2f) ? PerpHalf.front : PerpHalf.back;
    }

    // Imagine a line through origin cutting a plane in half, parallel to direction
    public enum ParaHalf { left, right };
    public static ParaHalf GetParaHalf(Vector2 Origin, Vector2 direction, Vector2 pnt)
    {
        return GetParaHalf(new Ray2(Origin, direction), pnt);
    }
    public static ParaHalf GetParaHalf(Ray2 dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.Origin;
        return (v.AngleTo(dir.Direction) <= 0f) ? ParaHalf.left : ParaHalf.right;
    }

    // Image a plane bisected by bath a parallel and perpendicular line thorugh origin
    public enum Halves {frontleft, frontright, backleft, backright}
    public static Halves GetHalves(Vector2 Origin, Vector2 direction, Vector2 pnt)
    {
        return GetHalves(new Ray2(Origin, direction), pnt);
    }
    public static Halves GetHalves(Ray2 dir, Vector2 pnt)
    {
        var parahalf = GetParaHalf(dir, pnt);
        var perphalf = GetPerpHalf(dir, pnt);
        if (parahalf == ParaHalf.left)
        {
            return perphalf == PerpHalf.front ? Halves.frontleft : Halves.backleft;
        }
        else
        {
            return perphalf == PerpHalf.front ? Halves.frontright : Halves.backright;
        }
    }


    // Imagine an X centered on origin, cutting a plane into four quarters
    public enum Quarter {front, back, left, right};

    public static Quarter GetQuarter(Vector2 origin, Vector2 direction, Vector2 pnt)
    {
        return GetQuarter(new Ray2(origin, direction), pnt);
    }
    public static Quarter GetQuarter(Ray2 dir, Vector2 pnt)
    {
        Vector2 v = pnt - dir.Origin;
        float ang = v.AngleTo(dir.Direction);
        float absAng = Mathf.Abs(Mathf.Rad2Deg(ang));
        Quarter ret;
        if (absAng <= 45.5f)
            ret = Quarter.front;
        else if(absAng >= 135f)
            ret = Quarter.back;
        else if(ang < 0)
            ret = Quarter.left;
        else
            ret = Quarter.right;

        return ret;
    }

    // Imagine a rectangle where lines are extended from the four corners outward at 45 degree angles
    public enum Facing {inside, front, back, left, right};

    public static Facing GetFacing(Vector2 origin, Vector2 direction, Vector2 pnt, float width, float height)
    {
        return GetFacing(new Ray2(origin, direction), pnt, width, height);
    }
    public static Facing GetFacing(Ray2 dir, Vector2 pnt, float frontage, float sideage)
    {
        var withinfontage = DistToLine(dir, pnt) < frontage/2f;
        var withinsideage = DistToLine(dir.Tangent(), pnt) < sideage/2f;
        if (withinfontage && withinsideage)
        {
            return Facing.inside;
        }

        throw new NotImplementedException();
    }

    // Imagine the lines making up a rectangle being extended ad infinitum, with the four corners being nulled out
    public enum Side {inside, none, front, back, left, right};
    public static Side GetSide(Vector2 origin, Vector2 direction, Vector2 pnt, float width, float height)
    {
        return GetSide(new Ray2(origin, direction), pnt, width, height);
    }

    public static Side GetSide(Ray2 dir, Vector2 pnt, float frontage, float sideage)
    {
        var fhalf = GetPerpHalf(dir, pnt);
        var shalf = GetParaHalf(dir, pnt);
        var withinfontage = DistToLine(dir, pnt) < frontage/2f;
        var withinsideage = DistToLine(dir.Tangent(), pnt) < sideage/2f;

        Side ret;
        if (withinfontage && withinsideage)
        {
            ret = Side.inside;
        }
        else if (withinfontage && fhalf == PerpHalf.front)
        {
            ret = Side.front;
        }
        else if (withinfontage && fhalf == PerpHalf.back)
        {
            ret = Side.back;
        }
        else if (withinsideage && shalf == ParaHalf.left)
        {
            ret = Side.left;
        }
        else if (withinsideage && shalf == ParaHalf.right)
        {
            ret = Side.right;
        }
        else
        {
            ret = Side.none;
        }

        return ret;
    }

    public static Vector2 LineIntersectionPoint(Ray2 l1, Ray2 l2)
    {
        return LineIntersectionPoint(l1.Origin, l1.Origin + l1.Direction, l2.Origin, l2.Origin + l2.Direction);
    }

    public static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
    {
        // Get A,B,C of first line - points : ps1 to pe1
        float A1 = pe1.y - ps1.y;
        float B1 = ps1.x - pe1.x;
        float C1 = A1 * ps1.x + B1 * ps1.y;

        // Get A,B,C of second line - points : ps2 to pe2
        float A2 = pe2.y - ps2.y;
        float B2 = ps2.x - pe2.x;
        float C2 = A2 * ps2.x + B2 * ps2.y;

        // Get delta and check if the lines are parallel
        float delta = A1 * B2 - A2 * B1;
        if (delta == 0)
            throw new System.Exception("Lines are parallel");

        // now return the Vector2 intersection point
        return new Vector2(
            (B2 * C1 - B1 * C2) / delta,
            (A1 * C2 - A2 * C1) / delta
        );
    }

    public static List<Vector2> SampleArc(Arc2 arc, int samples)
    {
        var pnts = new List<Vector2>();
        var seg = arc.Length/(float)samples;
        for (int i = 0; i < samples; i++)
        {
            pnts.Add(arc.GetPoint(seg * (float)i));
        }

        // Make sure last point is end
        var snapby = new Vector2(0.0001f, 0.0001f);
        if (pnts.Last().Snapped(snapby) != arc.End.Snapped(snapby))
        {
            pnts.Add(arc.End);
        }

        return pnts;
    }

    public static Vector2[] GetLineAsPolygon(Vector2[] pnts, float width)
    {
        var r_area_pnts = new List<Vector2>();
        var l_area_pnts = new List<Vector2>();
        var offset_v = Vector2.Zero;
        for (int i = 0; i < pnts.Length; i++)
        {
            // Use prev offset for final index
            if( i < pnts.Length - 1)
                offset_v = width * (pnts[i + 1] - pnts[i]).Normalized().Rotated(Mathf.Pi/2f);
            r_area_pnts.Add(pnts[i] + offset_v);
            l_area_pnts.Insert(0, pnts[i] - offset_v);
        }
        return r_area_pnts.Concat(l_area_pnts).ToArray();
    }

}
