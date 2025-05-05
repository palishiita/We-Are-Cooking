using System.Linq.Expressions;
using System.Reflection;

namespace RecipesAPI.Extensions
{
    public static class OrderByExtensions
    {
        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByProperty, bool asc)
        {
            string command = asc ? "OrderBy" : "OrderByDescending";
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },
                                          source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }

        public static IQueryable<TEntity> OrderByChildProperties<TEntity>(this IQueryable<TEntity> source, string childPropertyName, string orderByChildProperty, bool asc)
        {
            string command = asc ? "OrderBy" : "OrderByDescending";
            Type type = typeof(TEntity);

            var parameter = Expression.Parameter(type, "p");

            string[] parts = [childPropertyName, orderByChildProperty];
            Expression propertyAccess = parameter;
            Type currentType = type;

            foreach (var part in parts)
            {
                var property = currentType.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null)
                    throw new ArgumentException($"Property '{part}' not found on type '{currentType.Name}'");

                propertyAccess = Expression.Property(propertyAccess, property);
                currentType = property.PropertyType;
            }

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(
                typeof(Queryable),
                command,
                new Type[] { type, currentType },
                source.Expression,
                Expression.Quote(orderByExpression)
            );

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
