using System;

namespace VakunTranslatorVol2.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNull<T>(this T obj)
        {
            return obj == null;
        }
        public static bool IsNotNull<T>(this T obj)
        {
            return obj != null;
        }
        public static bool Is<T>(this IComparable<T> obj, T value)
        {
            return obj.CompareTo(value) == 0;
        }
    }
}