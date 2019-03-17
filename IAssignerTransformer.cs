using System;

namespace KazooDotNet.Utils
{
    public interface IAssignerTransformer
    {
        string Id { get; }
        (bool, object) Transform(Type targetType, object obj);
    }
}