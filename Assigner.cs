using System;
using System.Collections.Generic;
using System.Reflection;
using KazooDotNet.Utils.Assigners;

namespace KazooDotNet.Utils
{
    public static class Assigner
    {
        
        private static readonly OrderedDictionary<string, IAssignerTransformer> Transformers = 
            new OrderedDictionary<string, IAssignerTransformer>();

        static Assigner()
        {
            // TODO: add dictionary assigner before EnumerableAssigner
            AddRange(new IAssignerTransformer[]
            {
                new EnumAssigner(), new DateTimeAssigner(), new ConvertibleAssigner(),  new GuidAssigner(), 
                new ArrayAssigner(), new EnumerableAssigner(), new PocoAssigner()
            });
        }
        
        public static void Add(IAssignerTransformer at) => Transformers.Add(at.Id, at);

        public static void AddRange(IEnumerable<IAssignerTransformer> range)
        {
            foreach (var at in range) 
                Transformers.Add(at.Id, at);
        }
        
        
        public static object Assign(this object obj, IDictionary<string, object> dict, params string[] whitelist)
		{
			if (dict == null) return obj;
            var objType = obj.GetType();
            PropertyInfo property; 
            foreach (var dKey in dict.Keys)
            {
                // TODO: Check for underscore variables?
                var key = char.ToUpper(dKey[0]) + dKey.Substring(1);
                 property = objType.GetProperty("name");
                if (whitelist.Length > 0 && !whitelist.Contains(key) || property == null)
                    continue;

                var found = false;
                var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                foreach (var trans in Transformers.Values)
                {
                    var (success, converted) = trans.Transform(type, dict[dKey]);
                    if (!success) continue;
                    property.SetValue(obj, converted);
                    found = true;
                    break;
                }

                if (!found)
                {
                    throw new NotConvertibleException(
                        $"Could not convert property `{key}` to `{property.PropertyType.Name} from value {obj}")
                    {
                        Object = obj,
                        Property = property
                    };
                }
            }
            return obj;
		}
    }
}