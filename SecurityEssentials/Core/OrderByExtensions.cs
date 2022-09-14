using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SecurityEssentials.Core
{
	public static class OrderByExtensions
    {
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, string orderBy)
        {
            return enumerable.AsQueryable().OrderBy(orderBy).AsEnumerable();
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> collection, string orderBy)
        {
            foreach (OrderByInfo orderByInfo in ParseOrderBy(orderBy))
                collection = ApplyOrderBy(collection, orderByInfo);

            return collection;
        }

        private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> collection, OrderByInfo orderByInfo)
        {
            string[] props = orderByInfo.PropertyName.Split('.');
            Type type = typeof(T);

            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            string methodName = String.Empty;

            if (!orderByInfo.Initial && collection is IOrderedQueryable<T>)
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "ThenBy";
                else
                    methodName = "ThenByDescending";
            }
            else
            {
                if (orderByInfo.Direction == SortDirection.Ascending)
                    methodName = "OrderBy";
                else
                    methodName = "OrderByDescending";
            }

            return (IOrderedQueryable<T>)typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
                                                            .MakeGenericMethod(typeof(T), type)
                                                            .Invoke(null, new object[] { collection, lambda });
        }

        private static IEnumerable<OrderByInfo> ParseOrderBy(string orderBy)
        {
            if (String.IsNullOrEmpty(orderBy))
                yield break;

            string[] items = orderBy.Split(',');
            bool initial = true;
            foreach (string item in items)
            {
                string[] pair = item.Trim().Split(' ');

                if (pair.Length > 2)
                    throw new ArgumentException(
                        String.Format(
                            "Invalid OrderBy string '{0}'. Order By Format: Property, Property2 ASC, Property2 DESC",
                            item));

                string prop = pair[0].Trim();

                if (String.IsNullOrEmpty(prop))
                    throw new ArgumentException(
                        "Invalid Property. Order By Format: Property, Property2 ASC, Property2 DESC");

                var dir = SortDirection.Ascending;

                if (pair.Length == 2)
                    dir = ("desc".Equals(pair[1].Trim(), StringComparison.OrdinalIgnoreCase)
                               ? SortDirection.Descending
                               : SortDirection.Ascending);

                yield return new OrderByInfo { PropertyName = prop, Direction = dir, Initial = initial };

                initial = false;
            }
        }

        private class OrderByInfo
        {
            public string PropertyName { get; set; }
            public SortDirection Direction { get; set; }
            public bool Initial { get; set; }
        }

        private enum SortDirection
        {
            Ascending = 0,
            Descending = 1
        }
    }
}