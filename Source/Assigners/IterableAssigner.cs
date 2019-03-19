using System;

namespace KazooDotNet.Utils.Assigners
{
    public abstract class IterableAssigner
    {
        protected object ConvertValue(object value, Type toType, object obj, int index)
        {
            var (success, converted) = Assigner.Convert(value, toType);
            if (!success)
                throw new NotConvertibleException($"Cannot convert {value.GetType().Name} to {toType.Name}")
                {
                    Object = obj,
                    Index = index
                };
            return converted;
        }

        protected bool CanCreate(Type type) =>
            !(type?.IsAbstract ?? true) && (
                typeof(IConvertible).IsAssignableFrom(type) ||
                type.GetConstructor(Type.EmptyTypes) == null);
    }
}