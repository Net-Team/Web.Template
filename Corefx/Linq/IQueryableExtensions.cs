using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq
{
    /// <summary>
    /// 查询对象扩展
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// 排序公共方法
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderByKey">排序键排序键(不分大小写)</param>
        /// <param name="orderByMethod">排序方法</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        private static IOrderedQueryable<T> OrderByInternal<T>(this IQueryable<T> source, string orderByKey, string orderByMethod) where T : class
        {
            if (string.IsNullOrEmpty(orderByKey))
            {
                throw new ArgumentNullException(nameof(orderByKey));
            }

            var sourceType = typeof(T);
            var keyProperty = Property
                .GetProperties(sourceType)
                .FirstOrDefault(p => p.Name.EqualsIgnoreCase(orderByKey));

            if (keyProperty == null)
            {
                throw new ArgumentException($"{nameof(orderByKey)}不存在...");
            }

            var param = Expression.Parameter(sourceType, "item");
            var body = Expression.Property(param, keyProperty.Info);
            var orderByLambda = Expression.Lambda(body, param);

            var resultExp = Expression.Call(typeof(Queryable), orderByMethod, new Type[] { sourceType, keyProperty.Info.PropertyType }, source.Expression, Expression.Quote(orderByLambda));
            return source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
        }

        /// <summary>
        /// 排序
        /// 选择升序或降序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderByKey">排序键排序键(不分大小写)</param>
        /// <param name="ascending">是否升序</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByKey, bool ascending) where T : class
        {
            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            return source.OrderByInternal(orderByKey, methodName);
        }

        /// <summary>
        /// 排序次项
        /// 选择升序或降序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderByKey">排序键(不分大小写)</param>
        /// <param name="ascending">是否升序</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string orderByKey, bool ascending) where T : class
        {
            var methodName = ascending ? "ThenBy" : "ThenByDescending";
            return source.OrderByInternal(orderByKey, methodName);
        }


        /// <summary>
        /// 多字段混合排序       
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderByString">排序字符串：例如CreateTime desc, ID asc 不区分大小写</param>
        /// <exception cref="ArgumentNullException">orderByString</exception>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByString) where T : class
        {
            bool descFun(string[] item)
            {
                return item.Length > 1 && item[1].EqualsIgnoreCase("desc");
            }

            if (orderByString.IsNullOrEmpty() == true)
            {
                throw new ArgumentNullException(nameof(orderByString));
            }

            var parameters = orderByString
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                .Select(item => new { Key = item.FirstOrDefault(), Asc = !descFun(item) })
                .ToArray();

            if (parameters.Length == 0)
            {
                throw new ArgumentNullException(nameof(orderByString));
            }

            var first = parameters.FirstOrDefault();
            var orderQuery = source.OrderBy(first.Key, first.Asc);
            parameters.Skip(1).ForEach(p => orderQuery = orderQuery.ThenBy(p.Key, p.Asc));

            return orderQuery;
        }

        /// <summary>
        /// 排序
        /// 选择升序或降序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">排序键</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderKeySelector">排序器</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> orderKeySelector, bool ascending)
        {
            return ascending ? source.OrderBy(orderKeySelector) : source.OrderByDescending(orderKeySelector);
        }

        /// <summary>
        /// 次项排序
        /// 选择升序或降序
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TKey">排序键</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderKeySelector">排序器</param>
        /// <param name="ascending">是否升序</param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T, TKey>(this IOrderedQueryable<T> source, Expression<Func<T, TKey>> orderKeySelector, bool ascending)
        {
            return ascending ? source.ThenBy(orderKeySelector) : source.ThenByDescending(orderKeySelector);
        }
    }
}
