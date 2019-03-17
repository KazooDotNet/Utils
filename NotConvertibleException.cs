using System;
using System.Reflection;

namespace KazooDotNet.Utils
{
    public class NotConvertibleException : Exception
    {
        public object Object { get; set; }
        public PropertyInfo Property { get; set; }

        public NotConvertibleException()
        {
        }
    

        public NotConvertibleException(string msg) : base(msg)
        {
        }
    }
}