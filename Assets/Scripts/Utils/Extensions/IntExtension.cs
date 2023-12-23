using System;

namespace Utils.Extensions
{
    public static class IntExtension
    {
        public static void Iterate(this int i, Action<int> action)
        {
            var index = 0;
            while (index < i)
                action?.Invoke(index++);
        }

        public static void IterateTo(this int from, int to, Action<int> action)
        {
            if (from == to)
                return;
            
            var index = from;
            if (from < to)
                while (index <= to)
                    action?.Invoke(index++);
            else
                while (index >= to)
                    action?.Invoke(index--);
        }
        
        public static void IterateDescending(this int i, Action<int> action)
        {
            while (i >= 0)
                action?.Invoke(i--);
        }
    }
}