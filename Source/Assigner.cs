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
        
        // TODO: add other methods for dictionary
        
        
        public static object Assign(this object obj, IDictionary<string, object> dict, params string[] whitelist)
		{
			if (dict == null) return obj;
            var objType = obj.GetType();
            foreach (var dKey in dict.Keys)
            {
                // TODO: Check for underscore variables? Custom key transformers?
                var key = char.ToUpper(dKey[0]) + dKey.Substring(1);
                var property = objType.GetProperty(key);
                if (whitelist.Length > 0 && !whitelist.Contains(key) || property == null)
                    continue;
                var value = dict[dKey];
                if (value == null)
                {
                    property.SetValue(obj, null);
                    continue;
                }
                var (found, converted) = Convert(value, property.GetType());
                if (!found)
                    throw new NotConvertibleException(
                        $"Could not convert property `{key}` to `{property.PropertyType.FullName} from value {value}({value.GetType().FullName})")
                    {
                        Object = obj,
                        Property = property
                    };
                property.SetValue(obj, converted);
            }
            return obj;
		}

        public static (bool, object) Convert(object value, Type toType)
        {
            var type = Nullable.GetUnderlyingType(toType) ?? toType;
            if (type == value.GetType())
                return (true, value);
            foreach (var trans in Transformers.Values)
            {
                var (success, converted) = trans.Transform(type, value);
                if (!success) continue;
                return (true, converted);
            }
            return (false, null);
        }
    }
}