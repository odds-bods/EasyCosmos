using System;
using System.Linq.Expressions;

namespace EasyCosmos
{
    class ObjectActivator
    {
        public static object CreateInstance(Type expectedType)
        {
            return Expression.Lambda<Func<object>>(Expression.New(expectedType)).Compile().Invoke();
        }
    }
}
