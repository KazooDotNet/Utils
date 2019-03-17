using System;
using System.Collections.Generic;

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

            if (!(obj is Dictionary<string, object>[] aDict)) 
                return (false, null);
            var ret = (Array) Activator.CreateInstance(targetType, aDict.Length);
            var i = 0;
            foreach (var ad in aDict)
            {
                var arrayEle = Activator.CreateInstance(eleType);
                arrayEle.Assign(ad);
                ret.SetValue(arrayEle, i++);
            }
            return (true, ret);

        }
    }
}