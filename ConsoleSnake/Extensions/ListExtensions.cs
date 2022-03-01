using System.Collections.Generic;

namespace ConsoleSnake.Extensions
{
    public static class ListExtensions
    {
        public static T PopAt<T>(this IList<T> list, int i)
        {
            var value = list[i];
            list.RemoveAt(i);
            return value;
        }
    }
}