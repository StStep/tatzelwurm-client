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
            if(GetQuarter(startRay, end, 0, 0) != Quarter.front)
                throw new ArgumentException("Outside bounds");

            Start =  startRay.Origin;
            StartDir = startRay.Direction;
            End = end;

            // Base direction is a chord, and froms base of iso triangle
            var baseDir = (End - Start).Normalized();
            bool clockwise = StartDir.AngleTo(baseDir) > 0f;

            // LegA of iso triangle is perp to StartDir (which is a tangent), and it depends on direction, points toward center
            var legADir = clockwise ? new Vector2(-StartDir.y, StartDir.x) : new Vector2(StartDir.y, -StartDir.x);

            // LegB is perp to tangent at end point, and can be found because it has same corner angle as legA, points toward center
            var legBDir = (-baseDir).Rotated(legADir.AngleTo(baseDir));

            // EndDir is perp to legBDir, and it is a tangent, points toward rotation direction
            EndDir = clockwise ? new Vector2(legBDir.y, -legBDir.x) : new Vector2(-legBDir.y, legBDir.x);

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

    public enum Half { front, back };
    public static Half GetHalf(Vector2 Origin, Vector2 direction, Vector2 pnt, float frontage, float sideage)
    {
        return GetHalf(new Ray2(Origin, direction), pnt, frontage, sideage);
    }
    public static Half GetHalf(Ray2 dir, Vector2 pnt, float frontage, float sideage)
    {
        Half ret;

        Vector2 v = pnt - dir.Origin;
        float ang = v.AngleTo(dir.Direction);
        float absAng = Mathf.Abs(Mathf.Rad2Deg(ang));
        if (absAng <= 90f)
            ret = Half.front;
        else
            ret = Half.back;

        return ret;
    }

    public enum Quarter {front, back, left, right};
    public static Quarter GetQuarter(Vector2 origin, Vector2 direction, Vector2 pnt, float frontage, float sideage)
    {
        return GetQuarter(new Ray2(origin, direction), pnt, frontage, sideage);
    }
    public static Quarter GetQuarter(Ray2 dir, Vector2 pnt, float frontage, float sideage)
    {
        Quarter ret;

        Vector2 v = pnt - dir.Origin;
        float ang = v.AngleTo(dir.Direction);
        float absAng = Mathf.Abs(Mathf.Rad2Deg(ang));
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
