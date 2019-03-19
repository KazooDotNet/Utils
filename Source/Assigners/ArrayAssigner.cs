using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace KazooDotNet.Utils.Assigners
{
    public class ArrayAssigner : IAssignerTransformer
    {
        public string Id => "ArrayAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (!targetType.IsArray)
                return (false, null);
            var eleType = targetType.GetElementType();
            if (eleType?.GetConstructor(Type.EmptyTypes) == null || eleType.IsAbstract)
                return (false, null);
            
            switch (obj)
            {
                case IList l:
                    var a1 = (Array) Activator.CreateInstance(targetType, l.Count);
                    var i1 = 0;
                    foreach (var le in l)
                        a1.SetValue(ConvertValue(le, eleType, obj, i1), i1++);
                    return (true, a1);
                case IEnumerable e: // Slow catch all
                    var eType = e.GetType().GetGenericArguments()[0];
                    var list = (IList) Activator.CreateInstance(typeof(List<>).MakeGenericType(eType));
                    foreach (var ei in e)
                        list.Add(ei);
                    var a2 = (Array) Activator.CreateInstance(targetType, list.Count);
                    var i2 = 0;
                    foreach (var le in list)
                        a2.SetValue(ConvertValue(le, eleType, obj, i2), i2++);
                    return (true, a2);
                default:
                    return (false, null);
            }
        }
        

        private object ConvertValue(object value, Type toType, object obj, int index)
        {
            var (success, converted) = Assigner.Convert(value, toType);
            if (!success)
                throw new NotConvertibleException($"Cannot convert {value.GetType().Name} to {toType.Name}")
                {
                    Object = obj,
                    Index = index
                };
            return converted;
        }
    }
}