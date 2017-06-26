using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakunTranslatorVol2.Extensions
{
    public static class StackExtensions
    {
        public static IEnumerable<T> PopWhile<T>(this Stack<T> source, Func<T, bool> predicate)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            var buffer = new List<T>();
            T element = default(T);
            bool haveToPushLast = false;

            while (source.Count > 0)
            {
                if (!predicate(element = source.Pop()))
                {
                    haveToPushLast = true;
                    break;
                }

                buffer.Add(element);
            }

            if (haveToPushLast)
            {
                source.Push(element);
            }

            return buffer;
        }
    }
}
