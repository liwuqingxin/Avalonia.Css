using System.Reflection;

namespace Nlnet.Avalonia.Css.Test
{
    internal static class Utils
    {
        public static object? Invoke(this Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            return method.Invoke(type, args);
        }

        public static MethodInfo GetNonPublicStaticMethod(this Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            return method;
        }
    }
}
