using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Trig
{
    /// <summary>
    /// </summary>
    public static class Utility
    {
        public static float DistToLine(Ray dir, Vector2 pnt)
        {
            Vector2 linepnt = NearestPointOnLine(dir, pnt);
            return linepnt.DistanceTo(pnt);
        }

        public static Vector2 NearestPointOnLine(Ray dir, Vector2 pnt)
        {
            Vector2 v = pnt - dir.Origin;
            float d = v.Dot(dir.Direction);
            return dir.GetPoint(d);
        }

        // Imagine a line through origin cutting a plane in half, perpendicular to direction
        public enum PerpHalf { front, back };
        public static PerpHalf GetPerpHalf(Vector2 Origin, Vector2 direction, Vector2 pnt)
        {
            return GetPerpHalf(new Ray(Origin, direction), pnt);
        }
        public static PerpHalf GetPerpHalf(Ray dir, Vector2 pnt)
        {
            Vector2 v = pnt - dir.Origin;
            return (Mathf.Abs(v.AngleTo(dir.Direction)) <= Mathf.Pi / 2f) ? PerpHalf.front : PerpHalf.back;
        }

        // Imagine a line through origin cutting a plane in half, parallel to direction
        public enum ParaHalf { left, right };
        public static ParaHalf GetParaHalf(Vector2 Origin, Vector2 direction, Vector2 pnt)
        {
            return GetParaHalf(new Ray(Origin, direction), pnt);
        }
        public static ParaHalf GetParaHalf(Ray dir, Vector2 pnt)
        {
            return (dir.Direction.AngleTo(pnt - dir.Origin) <= 0f) ? ParaHalf.left : ParaHalf.right;
        }

        // Image a plane bisected by both a parallel and perpendicular line thorugh origin
        public enum Halves {frontleft, frontright, backleft, backright}
        public static Halves GetHalves(Vector2 Origin, Vector2 direction, Vector2 pnt)
        {
            return GetHalves(new Ray(Origin, direction), pnt);
        }
        public static Halves GetHalves(Ray dir, Vector2 pnt)
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

        public static float GetRotation(Quarter quarter)
        {
            switch(quarter)
            {
                case Quarter.front:
                    return 0f;
                case Quarter.back:
                    return Mathf.Pi;
                case Quarter.left:
                    return 3 * Mathf.Pi/2f;
                default:
                    return Mathf.Pi/2f;
            }
        }

        public static Vector2 GetDirection(Quarter quarter)
        {
            switch(quarter)
            {
                case Quarter.front:
                    return Vector2.Right;
                case Quarter.back:
                    return Vector2.Left;
                case Quarter.left:
                    return Vector2.Up;
                default:
                    return Vector2.Down;
            }
        }

        public static Quarter GetQuarter(Vector2 origin, float rotation, Vector2 pnt)
        {
            return GetQuarter(new Ray(origin, rotation), pnt);
        }

        public static Quarter GetQuarter(Vector2 origin, Vector2 direction, Vector2 pnt)
        {
            return GetQuarter(new Ray(origin, direction), pnt);
        }

        public static Quarter GetQuarter(Ray dir, Vector2 pnt)
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
            return GetFacing(new Ray(origin, direction), pnt, width, height);
        }
        public static Facing GetFacing(Ray dir, Vector2 pnt, float frontage, float sideage)
        {
            var halfFrontage = frontage / 2f;
            var halfSidage = sideage / 2f;
            var withinfontage = DistToLine(dir, pnt) < halfFrontage;
            var withinsideage = DistToLine(dir.Tangent(), pnt) < halfSidage;
            if (withinfontage && withinsideage)
            {
                return Facing.inside;
            }

            var half = GetHalves(dir, pnt);
            if (half == Halves.frontleft)
            {
                var flCorner = dir.RelTranslate(Vector2.Right * halfSidage + Vector2.Up * halfFrontage).Rotated(-Mathf.Pi / 4);
                return flCorner.AngleToPoint(pnt) < 0 ? Facing.left : Facing.front;
            }
            else if (half == Halves.frontright)
            {
                var frCorner = dir.RelTranslate(Vector2.Right * halfSidage + Vector2.Down * halfFrontage).Rotated(Mathf.Pi / 4);
                return frCorner.AngleToPoint(pnt) < 0 ? Facing.front : Facing.right;
            }
            else if (half == Halves.backleft)
            {
                var blCorner = dir.RelTranslate(Vector2.Left * halfSidage + Vector2.Up * halfFrontage).Rotated(-Mathf.Pi * 3 / 4);
                return blCorner.AngleToPoint(pnt) < 0 ? Facing.back : Facing.left;
            }
            else
            {
                var brCorner = dir.RelTranslate(Vector2.Left * halfSidage + Vector2.Down * halfFrontage).Rotated(Mathf.Pi * 3 / 4);
                return brCorner.AngleToPoint(pnt) < 0 ? Facing.right : Facing.back;
            }
        }

        // Imagine the lines making up a rectangle being extended ad infinitum, with the four corners being nulled out
        public enum Side {inside, none, front, back, left, right};
        public static Side GetSide(Vector2 origin, Vector2 direction, Vector2 pnt, float width, float height)
        {
            return GetSide(new Ray(origin, direction), pnt, width, height);
        }

        public static Side GetSide(Ray dir, Vector2 pnt, float frontage, float sideage)
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

        public static Vector2 LineIntersectionPoint(Ray l1, Ray l2)
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

        public static List<Vector2> SampleArc(Arc arc, int samples)
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

        // Return the area under a right trapazoid, where the right angles make the base of the shape
        // Treated as a right triangle on top of a rectangle, with width w and height the smaller of h1 or h2
        public static float AreaUnderRightTrapezoid(float h1, float h2, float w)
        {
            if (h2 > h1)
            {
                return (h2 - h1) * w * 0.5f + h1 * w;
            }
            else
            {
                return (h1 - h2) * w * 0.5f + h2 * w;
            }
        }

        // Return the area under a line with negative slope passing through a point on the y-axis with value y
        // Math: m,h > 0
        // Right triangle on origin with height h and width w, line with given slope m makes up hypotenuse has equation y = -m*x + h
        // Line crosses axis at w, with 0 = -m*w + h, w = h/m
        // Area under line A = 0.5 * w * h = 0.5 * h/m * h = h^2/(2*m)
        public static float AreaUnderDownRamp(float h, float m) => h * h / (2f * m);

        // Return the Height for a ramp with a given negative slope at a point with the given remaining area under ramp
        // Math: m,A > 0
        // Area under line A = h^2/(2*m)
        // h = sqrt(A*2*m)
        public static float HeightOnDownRamp(float m, float A) => Mathf.Sqrt(2f* m * A);

        // Return the height at which two lines with given slopes produce a triangle containing given area and initial point at x = 0, y_1
        // Math:
        // A = a + b + c
        // a = y_1 * x_1
        // b = (y_2 - y_1) * x_1 * 0.5
        // c = (x_2 - X_1) * y_2 * 0.5
        // y_2 = m_1 * x_1 + y_1; x_1 = (y_2 - y_1) / m_1
        // y_2 = -m_2 * x_1 + m_2 * x_2; x_2 = (y_2 + m_2 * x_1) / m_2
        // y_2 = Mathf.Sqrt(2 * m_1 * A + y_1 * y_1) / Mathf.Sqrt(m_1 / m_2 + 1)
        public static float HeightOfTriWithGivenSlopesArea(float y_1, float m_1, float m_2, float A) => Mathf.Sqrt(2 * m_1 * A + y_1 * y_1) / Mathf.Sqrt(m_1 / m_2 + 1);
    }
}
