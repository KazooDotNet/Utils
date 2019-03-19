using System;
using System.Collections.Generic;

namespace KazooDotNet.Utils.Assigners
{
    public class PocoAssigner : SharedAssigner, IAssignerTransformer
    {
        public string Id => "PocoAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            
            if (targetType.GetConstructor(Type.EmptyTypes) == null || targetType.IsAbstract || !(obj is IDictionary<string,object> dict))
                return (false, null);
            var ret = Activator.CreateInstance(targetType);
            ret.Assign(dict);
            return (true, ret);
        }
    }
}