using System;
using System.Collections;
using System.Collections.Generic;

namespace KazooDotNet.Utils.Assigners
{
    public class EnumerableAssigner : IterableAssigner, IAssignerTransformer
    {
        public string Id => "EnumerableAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(targetType) || !(obj is IEnumerable e))
                return (false, null);
            var eleType = targetType.GetGenericArguments()[0];
            if (!CanCreate(eleType))
                return (false, null);
            var typedList = typeof(List<>).MakeGenericType(eleType);
            var l = (IList) Activator.CreateInstance(typedList);
            var i = 0;
            foreach (var ev in e)
                l.Add(ConvertValue(ev, eleType, obj, i++));
            return (true, l);
        }
    }
}