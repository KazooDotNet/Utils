using System;

namespace KazooDotNet.Utils.Assigners
{
    public class DateTimeAssigner : IAssignerTransformer
    {
        public string Id => "DateTimeAssigner";
        public (bool, object) Transform(Type targetType, object obj)
        {
            if (targetType != typeof(DateTime))
                return (false, null);
            switch (obj)
            {
                case string s:
                    DateTime.TryParse(s, out var dt);
                    return (true, dt);
                case int i:
                    return (true, new DateTime(i));
                
                default:
                    return (false, null);
            }
        }
    }
}