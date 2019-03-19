using System;
using System.Collections;
using System.Collections.Generic;

namespace KazooDotNet.Utils.Assigners
{
    public class EnumerableAssigner : IAssignerTransformer
    {
        public string Id => "EnumerableAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(targetType) || !(obj is Dictionary<string, object>[] aDict))
                return (false, null);
            var eleType = targetType.GetGenericArguments()[0];
            if (eleType.GetConstructor(Type.EmptyTypes) == null || eleType.IsAbstract)
                return (false, null);
            var listType = typeof(List<>).MakeGenericType(eleType);
            // TODO: make list specific size?
            var l = (IList) Activator.CreateInstance(listType);
            foreach (var ad in aDict)
            {
                var arrayEle = Activator.CreateInstance(eleType);
                arrayEle.Assign(ad);
                l.Add(arrayEle);
            }
            return (true, l);
        }
    }
}