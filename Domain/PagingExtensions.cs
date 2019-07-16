using Core.Paging;
using Microsoft.EntityFrameworkCore;
using PredicateLib;
using System;
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
        /// <param name="source">数据源</param>    
        /// <param name="orderBy">排序字符串</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="idSelector">Id选择器</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T, TId>(this IQueryable<T> source, string orderBy, int pageIndex, int pageSize, Expression<Func<T, TId>> idSelector)
            where T : class
            where TId : class
        {
            source = source.Where(Predicate.Create(idSelector, null, Operator.NotEqual));
            int total = await source.CountAsync();
            var inc = total % pageSize > 0 ? 0 : -1;
            var maxPageIndex = (int)Math.Floor((double)total / pageSize) + inc;
            pageIndex = Math.Max(0, Math.Min(pageIndex, maxPageIndex));

            var idQuery = source.OrderBy(orderBy).Skip(pageIndex * pageSize).Take(pageSize).Select(idSelector);
            var datas = await source.Join(idQuery, idSelector, item => item, (item, id) => item).OrderBy(orderBy).ToArrayAsync();

            return new Page<T>(datas) { PageIndex = pageIndex, PageSize = pageSize, TotalCount = total };
        }

        /// <summary>
        /// 执行分页        
        /// 性能比较好
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源</param>    
        /// <param name="orderBy">排序字符串</param>
        /// <param name="pageIndex">分页索引</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns></returns>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> source, string orderBy, int pageIndex, int pageSize) where T : class
        {
            int total = await source.CountAsync();
            var inc = total % pageSize > 0 ? 0 : -1;
            var maxPageIndex = (int)Math.Floor((double)total / pageSize) + inc;
            pageIndex = Math.Max(0, Math.Min(pageIndex, maxPageIndex));

            var datas = await source.OrderBy(orderBy).Skip(pageIndex * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            return new Page<T>(datas) { PageIndex = pageIndex, PageSize = pageSize, TotalCount = total };
        }
    }
}
