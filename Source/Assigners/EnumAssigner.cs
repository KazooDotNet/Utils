using System;

namespace KazooDotNet.Utils.Assigners
{
    public class EnumAssigner : IAssignerTransformer
    {
        public string Id => "EnumAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (!targetType.IsEnum)
                return (false, null);
            try
            {
                return (true, Enum.Parse(targetType, obj.ToString(), true));
            }
            catch
            {
                return (false, null);
            }
        }
    }
}