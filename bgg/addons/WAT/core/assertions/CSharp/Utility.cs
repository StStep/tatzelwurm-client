using System;
using Godot;
using Object = Godot.Object;

namespace WAT
{
    public class Utility: Assertion
    {
        public static object Fail(string context)
        {
            return Result(false, "N/A", "N/A", context);
        }
    }
}
