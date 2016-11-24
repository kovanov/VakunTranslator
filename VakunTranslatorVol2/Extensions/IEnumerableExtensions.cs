using System;
using System.Collections.Generic;
using System.Linq;

namespace VakunTranslatorVol2.Extensions
{
    public static class IEnumerableExtensions
    {
        public static List<List<T>> SplitAfter<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var enumerator = source.GetEnumerator();
            var result = new List<List<T>>();
            var moveNext = true;

            while(moveNext)
            {
                var inner = new List<T>();

                while((moveNext = enumerator.MoveNext()))
                {
                    var current = enumerator.Current; 

                    inner.Add(current);

                    if(predicate(current))
                    {
                        break;
                    }
                }
                result.Add(inner);
            }
            return result;
        }

        public static bool StartsWith<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var enumerator = source.GetEnumerator();

            return enumerator.MoveNext() && predicate(enumerator.Current);
        }
    }
}