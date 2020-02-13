using System;
using System.Collections.Generic;

namespace KazooDotNet.Utils.Assigners
{
    public class PocoAssigner : SharedAssigner, IAssignerTransformer
    {
        public string Id => "PocoAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            
            if (targetType.GetConstructor(Type.EmptyTypes) == null || targetType.IsAbstract || !(obj is IDictionary<string,object> || obj is IEnumerable<KeyValuePair<string,object>>))
                return (false, null);
            var ret = Activator.CreateInstance(targetType);
            
            if (obj is IDictionary<string,object> dict)
                ret.Assign(dict);
            else if (obj is IEnumerable<KeyValuePair<string, object>> kvps)
                ret.Assign(kvps);
            return (true, ret);
        }
    }
}