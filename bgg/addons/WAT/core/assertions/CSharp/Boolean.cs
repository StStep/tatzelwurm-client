using System.Dynamic;
using Godot;

namespace WAT
{
	public class Boolean: Assertion
	{
		public static object IsTrue(bool value, string context)
		{
			var passed = $"|boolean| {value.ToString()} == true";
			var failed = $"|boolean| {value.ToString()} == false";
			var success = value;
			var result = success ? passed : failed;
			return Result(success, passed, result, context);
		}

		public static object IsFalse(bool value, string context)
		{
			var passed = $"|boolean| {value.ToString()} == false";
			var failed = $"|boolean| {value.ToString()} == true";
			var success = !value;
			var result = success ? passed : failed;
			return Result(success, passed, result, context);
		}
	}
}

