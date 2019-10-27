using System;
using Godot;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class TrigTests
    {
        [TestMethod]
        public void ArcBasic()
        {
            var arc = new Trig.Arc2(new Vector2(480.524f, 407.198f), 0f, new Vector2(401f, 177f));
        }
    }
}
