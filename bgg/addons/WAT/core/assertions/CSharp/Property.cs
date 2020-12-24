namespace WAT
{
    public class Property: Assertion
    {
        public static object Contains<T>(object value, T container, string context)
        {
            var passed = $"{container.GetType()} contains |{value.GetType()}|{value}";
            var failed = $"{container.GetType()} does not contain |{value.GetType()}|{value}";
            var method = container.GetType().GetMethod("Contains");
            var success = false;
            if (method != null)
            {
                success = (bool) method.Invoke(container, new object[] {value});
            }

            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }

        public static object DoesNotContain<T>(object value, T container, string context)
        {
            var passed = $"{container.GetType()} does not contain |{value.GetType()}|{value}";
            var failed = $"{container.GetType()} contains |{value.GetType()}|{value}";
            var method = container.GetType().GetMethod("Contains");
            var success = false;
            if (method != null)
            {
                success = !(bool) method.Invoke(container, new object[] {value});
            }

            var result = success ? passed : failed;
            return Result(success, passed, result, context);
        }
    }
}
