using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CSLibrary
{
    public static class Ext
    {
        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> seq, Expression<Func<T,U>> expression)
        {
            return seq.Select(expression.Compile());
        }
    }
}
