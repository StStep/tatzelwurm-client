using System;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace WAT
{
    public class Equality: Assertion
    {
        public static object IsEqual(object a, object b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is not equal to |{b.GetType()}|{b}";
            var success = (a.Equals(b));
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object IsNotEqual(object a, object b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is not equal to |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to |{b.GetType()}|{b}";
            var success = !(a.Equals(b));
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object IsEqualOrGreaterThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to or greater than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is less than |{b.GetType()}|{b}";
            var success = a >= b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object IsEqualOrLessThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is equal to or less than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is greater than |{b.GetType()}|{b}";
            var success = a <= b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object IsGreaterThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is greater than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to or less than |{b.GetType()}|{b}";
            var success = a > b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object IsLessThan(float a, float b, string context)
        {
            var passed = $"|{a.GetType()}| {a} is less than |{b.GetType()}|{b}";
            var failed = $"|{a.GetType()}| {a} is equal to or greater than |{b.GetType()}|{b}";
            var success = a < b;
            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }
    }
}