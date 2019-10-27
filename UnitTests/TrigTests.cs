using System;
using System.Collections.Generic;
using Godot;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TrigTests
    {
        public static IEnumerable<object[]> ArcBasicData()
        {
            yield return new object[] { Vector2.Zero, 0f, new Vector2(1f, -1f), new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, -1f), Vector2.Up, new Vector2(0f, -1f)), 1f, -Mathf.Pi / 2f, Mathf.Pi / 2f };
            yield return new object[] { Vector2.Zero, 0f, new Vector2(1f, 1f), new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, 1f), Vector2.Down, new Vector2(0f, 1f)), 1f, Mathf.Pi / 2f, Mathf.Pi / 2f };
            yield return new object[] { Vector2.Zero, 0f, new Vector2(0.7071f, -0.2929f), new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(0.7071f, -0.2929f), new Vector2(0.7071f, -0.7071f), new Vector2(0f, -1f)), 1f, -Mathf.Pi / 4f, Mathf.Pi / 4f };
        }

        [DataTestMethod]
        [DynamicData(nameof(ArcBasicData), DynamicDataSourceType.Method)]
        public void ArcBasic(Vector2 start, float startRot, Vector2 end, Trig.Arc2 exp, float radius, float angle, float length)
        {
            var arc = new Trig.Arc2(start, startRot, end);
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

        public static IEnumerable<object[]> ArcPntData()
        {
            yield return new object[] { new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, -1f), Vector2.Up, new Vector2(0f, -1f)), Mathf.Pi / 4f, new Vector2(0.7071f, -0.2929f) };
            yield return new object[] { new Trig.Arc2(Vector2.Zero, Vector2.Right, new Vector2(1f, 1f), Vector2.Down, new Vector2(0f, 1f)), Mathf.Pi / 4f, new Vector2(0.7071f, 0.2929f) };
        }

        [DataTestMethod]
        [DynamicData(nameof(ArcPntData), DynamicDataSourceType.Method)]
        public void ArcPnt(Trig.Arc2 arc, float dist, Vector2 expPnt)
        {
            Assert.AreEqual(expPnt, arc.GetPoint(dist).Snapped(new Vector2(0.0001f, 0.0001f)));
        }
    }
}
