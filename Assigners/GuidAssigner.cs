using System;

namespace KazooDotNet.Utils.Assigners
{
    public class GuidAssigner : IAssignerTransformer
    {
        public string Id => "GuidAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (!typeof(Guid).IsAssignableFrom(targetType))
                return (false, null);
            Guid.TryParse(obj.ToString(), out var guid);
            return (true, guid);
        }
    }
}