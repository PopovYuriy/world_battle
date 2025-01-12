using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.Extensions
{
    public static class CollectionExtension
    {
        public static T Random<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }
    }
}