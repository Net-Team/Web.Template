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
                throw new ArgumentNullException("orderByKey");
            }

            var sourceTtype = typeof(T);
            var keyProperty = Property
                .GetProperties(sourceTtype)
                .FirstOrDefault(p => p.Name.Equals(orderByKey, StringComparison.OrdinalIgnoreCase));

            if (keyProperty == null)
            {
                throw new ArgumentException("orderByKey不存在...");
            }

            var param = Expression.Parameter(sourceTtype, "item");
            var body = Expression.MakeMemberAccess(param, keyProperty.Info);
            var orderByLambda = Expression.Lambda(body, param);

            var resultExp = Expression.Call(typeof(Queryable), orderByMethod, new Type[] { sourceTtype, keyProperty.Info.PropertyType }, source.Expression, Expression.Quote(orderByLambda));
            var ordereQueryable = source.Provider.CreateQuery<T>(resultExp) as IOrderedQueryable<T>;
            return ordereQueryable;
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
            if (orderByString.IsNullOrEmpty())
            {
                throw new ArgumentNullException("orderByString");
            }

            Func<string[], bool> descFun = (item) => item.Length > 1 && item[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            var parameters = orderByString
                .Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => item.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                .Select(item => new { Key = item.FirstOrDefault(), Asc = !descFun(item) })
                .ToArray();

            if (parameters.Length == 0)
            {
                throw new ArgumentNullException("orderByString");
            }

            var firstP = parameters.FirstOrDefault();
            var orderQuery = source.OrderBy(firstP.Key, firstP.Asc);
            parameters.Skip(1).ToList().ForEach(p => orderQuery = orderQuery.ThenBy(p.Key, p.Asc));

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
            if (ascending)
            {
                return source.OrderBy(orderKeySelector);
            }
            else
            {
                return source.OrderByDescending(orderKeySelector);
            }
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
            if (ascending)
            {
                return source.ThenBy(orderKeySelector);
            }
            else
            {
                return source.ThenByDescending(orderKeySelector);
            }
        }
    }
}
