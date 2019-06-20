using System;
using System.Collections.Generic;

namespace KazooDotNet.Utils
{
    public static class ObjectExtensions
    {
        public static bool Contains<T>(this T[] thisArray, T searchElement) => 
            Array.Exists(thisArray, x => x.Equals(searchElement));
        
        public static int SequenceSearch<T>(this IReadOnlyList<T> haystack, IReadOnlyList<T> needle, int haystackIndex = 0) where T : IComparable<T>
        {
            var nCount = needle.Count;
            var hCount = haystack.Count;
            if (nCount > hCount)
                return -1;
            var needleIndex = 0;
            if (haystackIndex < 0) 
                haystackIndex = 0;
			
            while(haystackIndex < hCount)
            {
                var h = haystack[haystackIndex];
                var n = needle[needleIndex];
                if (n.CompareTo(h) == 0)
                    needleIndex++;
                else
                    needleIndex = 0;
                haystackIndex++;
                if (needleIndex == nCount)
                    return haystackIndex - nCount;
            }
            return -1;
        }
    }
}