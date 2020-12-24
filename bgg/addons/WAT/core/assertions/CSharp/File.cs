using System;
using Godot;

namespace WAT
{
    public class File: Assertion
    {
        public static object Exists(string path, string context)
        {
            var passed = $"{path} exists";
            var failed = $"{path} does not exist";
            var success = new Godot.File().FileExists(path);
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object DoesNotExist(string path, string context)
        {
            var passed = $"{path} does not exist";
            var failed = $"{path} exists";
            var success = !new Godot.File().FileExists(path);
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }
    }
}