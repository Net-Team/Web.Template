using Core;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 分页信息
    /// </summary>
    public static class PagingExtensions
    {
        /// <summary>
        /// 执行分页        
        /// 性能最好
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="TId">Id类型</typeparam>
        /// <param name="source">数据源</param>    
        /// <param name="orderBy">排序字符串</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="idSelector">Id选择器</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T, TId>([NotNull]this IQueryable<T> source, [NotNull] string orderBy, int pageIndex, int pageSize, [NotNull]Expression<Func<T, TId>> idSelector)
            where T : class
            where TId : class
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "值不能小于1");
            }

            source = source.Where(Predicate.Create(idSelector, null, Operator.NotEqual));
            int total = await source.CountAsync();
            var inc = total % pageSize > 0 ? 0 : -1;
            var maxPageIndex = (int)Math.Floor((double)total / pageSize) + inc;
            pageIndex = Math.Max(0, Math.Min(pageIndex, maxPageIndex));

            var idQuery = source.OrderBy(orderBy).Skip(pageIndex * pageSize).Take(pageSize).Select(idSelector);
            var datas = await source.Join(idQuery, idSelector, item => item, (item, id) => item).OrderBy(orderBy).ToArrayAsync();

            return new Page<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = total,
                DataArray = datas
            };
        }

        /// <summary>
        /// 执行分页        
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>    
        /// <param name="orderBy">排序字符串</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>([NotNull]this IQueryable<T> source, [NotNull] string orderBy, int pageIndex, int pageSize) where T : class
        {
            if (pageSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "值不能小于1");
            }

            int total = await source.CountAsync();
            var inc = total % pageSize > 0 ? 0 : -1;
            var maxPageIndex = (int)Math.Floor((double)total / pageSize) + inc;
            pageIndex = Math.Max(0, Math.Min(pageIndex, maxPageIndex));

            var datas = await source.OrderBy(orderBy).Skip(pageIndex * pageSize).Take(pageSize).AsNoTracking().ToArrayAsync();
            return new Page<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = total,
                DataArray = datas
            };
        }
    }
}
