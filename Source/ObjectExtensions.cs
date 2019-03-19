using System;

namespace KazooDotNet.Utils
{
    public static class ObjectExtensions
    {
        public static bool Contains<T>(this T[] thisArray, T searchElement) => 
            Array.Exists(thisArray, x => x.Equals(searchElement));
    }
}