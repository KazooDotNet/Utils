using System;

namespace KazooDotNet.Utils.Assigners
{
    public class ConvertibleAssigner : IAssignerTransformer
    {
        public string Id => "ConvertibleAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            return !typeof(IConvertible).IsAssignableFrom(targetType) ? 
                (false, null) : 
                (true, Convert.ChangeType(obj, targetType));
        }
    }
}