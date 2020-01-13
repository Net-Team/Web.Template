using Core;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application
{
    /// <summary>
    /// 表示基于数据库的抽象服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SqlDbContextService<T> : ScopedApplicationService where T : class, IStringIdable
    {
        /// <summary>
        /// 获取db上下文
        /// </summary>
        protected SqlDbContext Db { get; }

        /// <summary>
        /// 基于数据库的抽象服务
        /// </summary>
        /// <param name="db"></param>
        public SqlDbContextService(SqlDbContext db)
        {
            this.Db = db;
        }

        /// <summary>
        /// 检测是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Db.Set<T>().AnyAsync(where);
        }

        /// <summary>
        /// 根据Id获取记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ValueTask<T> FindAsync(string id)
        {
            return this.Db.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// 根据条件获取一条记录
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> FindAsync(Expression<Func<T, bool>> where)
        {
            return await this.Db.Set<T>().Where(where).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据Id获取记录
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Task<TNew> FindAsync<TNew>(string id, [NotNull]Expression<Func<T, TNew>> selector)
        {
            return this.Db.Set<T>().Where(item => item.Id == id).Select(selector).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 根据Id获取记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<T[]> FindManyAsync(IEnumerable<string> id)
        {
            var where = Predicate.CreateOrEqual<T, string>(item => item.Id, id);
            return this.FindManyAsync(where);
        }

        /// <summary>
        /// 根据条件获取记录
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<T[]> FindManyAsync([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Db.Set<T>().Where(where).ToArrayAsync();
        }

        /// <summary>
        /// 根据条件获取记录
        /// </summary>
        /// <param name="where"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public Task<TNew[]> FindManyAsync<TNew>([NotNull]Expression<Func<T, bool>> where, [NotNull]Expression<Func<T, TNew>> selector)
        {
            return this.Db.Set<T>().Where(where).Select(selector).ToArrayAsync();
        }

        /// <summary>
        /// 获取记录分页数据
        /// </summary>
        /// <param name="orderBy">排序</param>
        /// <param name="pageIndex">当前页码索引，从0开始</param>
        /// <param name="pageSize">每页数据</param>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public Task<Page<T>> GetPageAsync([NotNull]string orderBy, int pageIndex, int pageSize, [NotNull]Expression<Func<T, bool>> where)
        {
            return this.Db.Set<T>().Where(where).ToPageAsync(orderBy, pageIndex, pageSize);
        }


    }
}
