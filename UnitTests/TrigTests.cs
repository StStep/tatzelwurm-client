using System;
using System.Collections.Generic;
using Godot;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TrigTests
    {
        public Vector2 SnapBy => new Vector2(0.0001f, 0.0001f);

        public static IEnumerable<object[]> ArcBasicData()
        {
            yield return new object[] { Vector2.Zero, 0f, new Vector2(1f, -1f), new Trig.Arc(Vector2.Zero, Vector2.Right, new Vector2(1f, -1f), Vector2.Up, new Vector2(0f, -1f)), 1f, -Mathf.Pi / 2f, Mathf.Pi / 2f };
            yield return new object[] { Vector2.Zero, 0f, new Vector2(1f, 1f), new Trig.Arc(Vector2.Zero, Vector2.Right, new Vector2(1f, 1f), Vector2.Down, new Vector2(0f, 1f)), 1f, Mathf.Pi / 2f, Mathf.Pi / 2f };
            yield return new object[] { Vector2.Zero, 0f, new Vector2(0.7071f, -0.2929f), new Trig.Arc(Vector2.Zero, Vector2.Right, new Vector2(0.7071f, -0.2929f), new Vector2(0.7071f, -0.7071f), new Vector2(0f, -1f)), 1f, -Mathf.Pi / 4f, Mathf.Pi / 4f };
        }

        [DataTestMethod]
        [DynamicData(nameof(ArcBasicData), DynamicDataSourceType.Method)]
        public void ArcBasic(Vector2 start, float startRot, Vector2 end, Trig.Arc exp, float radius, float angle, float length)
        {
            var arc = new Trig.Arc(start, startRot, end);
            arc.Snap(new Vector2(0.0001f, 0.0001f));
            Assert.AreEqual(exp.Start, arc.Start);
            Assert.AreEqual(exp.StartDir, arc.StartDir);
            Assert.AreEqual(exp.End, arc.End);
            Assert.AreEqual(exp.EndDir, arc.EndDir);
            Assert.AreEqual(exp.Center, arc.Center);
            Assert.AreEqual(radius, arc.Radius, 0.0001f);
            Assert.AreEqual(angle, arc.Angle, 0.0001f);
            Assert.AreEqual(length, arc.Length, 0.0001f);
        }

        //    public static IEnumerable<object[]> ArcPntData()
        //    {
        //        yield return new object[] { new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, -1f), Vector2.Up, new Vector2(0f, -1f)), Mathf.Pi / 4f, new Vector2(0.7071f, -0.2929f) };
        //        yield return new object[] { new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, 1f), Vector2.Down, new Vector2(0f, 1f)), Mathf.Pi / 4f, new Vector2(0.7071f, 0.2929f) };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(ArcPntData), DynamicDataSourceType.Method)]
        //    public void ArcPnt(Trig.Arc2 arc, float dist, Vector2 expPnt)
        //    {
        //        Assert.AreEqual(expPnt, arc.GetPoint(dist).Snapped(SnapBy));
        //    }

        //    public static IEnumerable<object[]> Ray2Data()
        //    {
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    true, new Trig.Ray2(Vector2.Zero, Vector2.Down),
        //                                    new Vector2(1, 2), new Trig.Ray2(new Vector2(1, 2), Vector2.Right) };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Up),
        //                                    false, new Trig.Ray2(Vector2.Zero, Vector2.Left),
        //                                    new Vector2(1, 2), new Trig.Ray2(new Vector2(2, -1), Vector2.Up) };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(Ray2Data), DynamicDataSourceType.Method)]
        //    public void Ray2(Trig.Ray2 ray, Boolean tanIsClockwise, Trig.Ray2 expTanRay, Vector2 reloffset, Trig.Ray2 expOffsetRay)
        //    {
        //        Assert.AreEqual(expTanRay, ray.Tangent(tanIsClockwise).Snapped(SnapBy));
        //        Assert.AreEqual(expOffsetRay, ray.RelTranslate(reloffset).Snapped(SnapBy));
        //    }

        //    public static IEnumerable<object[]> ParaHalfData()
        //    {
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, -1f), Trig.ParaHalf.left };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, 1f), Trig.ParaHalf.right };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, -1f), Trig.ParaHalf.left };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, 1f), Trig.ParaHalf.right };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(ParaHalfData), DynamicDataSourceType.Method)]
        //    public void ParaHalf(Trig.Ray2 ray, Vector2 pnt, Trig.ParaHalf expParaHalf)
        //    {
        //        Assert.AreEqual(expParaHalf, Trig.GetParaHalf(ray, pnt));
        //    }

        //    public static IEnumerable<object[]> PerpHalfData()
        //    {
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, -1f), Trig.PerpHalf.front };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, 1f), Trig.PerpHalf.front };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, -1f), Trig.PerpHalf.back };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, 1f), Trig.PerpHalf.back };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(PerpHalfData), DynamicDataSourceType.Method)]
        //    public void PerpHalf(Trig.Ray2 ray, Vector2 pnt, Trig.PerpHalf expPerpHalf)
        //    {
        //        Assert.AreEqual(expPerpHalf, Trig.GetPerpHalf(ray, pnt));
        //    }

        //    public static IEnumerable<object[]> HalvesData()
        //    {
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, -1f), Trig.Halves.frontleft };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(1f, 1f), Trig.Halves.frontright };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, -1f), Trig.Halves.backleft };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    new Vector2(-1f, 1f), Trig.Halves.backright };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(HalvesData), DynamicDataSourceType.Method)]
        //    public void Halves(Trig.Ray2 ray, Vector2 pnt, Trig.Halves expHalf)
        //    {
        //        Assert.AreEqual(expHalf, Trig.GetHalves(ray, pnt));
        //    }

        //    public static IEnumerable<object[]> FacingData()
        //    {
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    150f, 50f, new Vector2(400f, 0f), Trig.Facing.front };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    150f, 50f, new Vector2(-400f, 0f), Trig.Facing.back };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    150f, 50f, new Vector2(0, -400f), Trig.Facing.left };
        //        yield return new object[] { new Trig.Ray2(Vector2.Zero, Vector2.Right),
        //                                    150f, 50f, new Vector2(0, 400f), Trig.Facing.right };
        //    }

        //    [DataTestMethod]
        //    [DynamicData(nameof(FacingData), DynamicDataSourceType.Method)]
        //    public void Facing(Trig.Ray2 ray, float frontage, float sideage, Vector2 pnt, Trig.Facing expFacing)
        //    {
        //        Assert.AreEqual(expFacing, Trig.GetFacing(ray, pnt, frontage, sideage));
        //    }
    }
}
