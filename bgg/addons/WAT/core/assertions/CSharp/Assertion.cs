using System.Diagnostics;
using System.Dynamic;
using Godot;

namespace WAT
{
    public class Assertion
    {
        private static readonly Script _Result = GD.Load<Script>("res://addons/WAT/core/assertions/GDScript/assertion_result.gd");

        protected static object Result(bool success, string expected, string actual, string context, string notes = "")
        {
            var x =  _Result.Call("new", success, expected, actual, context, notes);
            Debug.Assert(x != null, "Result Object Is Null");
            return x;
        }
    }
}

