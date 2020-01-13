using Microsoft.EntityFrameworkCore;
using QMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// Dbset扩展
    /// </summary>
    public static class DbSetExtensions
    {
        /// <summary>
        /// 将Value更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="set"></param>
        /// <param name="value">值</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static Task<T> UpdateFromAsync<T, TValue>(this DbSet<T> set, TValue value)
           where T : class
           where TValue : class, IStringIdable
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Id.IsNullOrEmpty())
            {
                throw new ArgumentException("value的Id值不为能null");
            }

            return set.UpdateFromAsync(value, value.Id);
        }

        /// <summary>
        /// 将Value更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="set"></param>
        /// <param name="value">值</param>
        /// <param name="id">id</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<T> UpdateFromAsync<T, TValue>(this DbSet<T> set, TValue value, string id)
            where T : class
            where TValue : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (id.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(id));
            }

            var local = await set.FindAsync(id);
            if (local != null)
            {
                value.AsMap().To(local);
            }
            return local;
        }

        /// <summary>
        /// 将Value更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="set"></param>
        /// <param name="value">值</param>
        /// <param name="id">id</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static async Task<T> UpdateEntityAsync<T, TValue>(this DbSet<T> set, TValue value, string id)
            where T : class, IEntity
            where TValue : class
        {
            var local = await set.UpdateFromAsync(value, id);
            local.LastModifyTime = DateTime.Now;
            return local;
        }

        /// <summary>
        /// 通过id标记记录为删除
        /// </summary>
        /// <param name="set"></param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public static async Task<T> RemoveAsync<T>(this DbSet<T> set, [NotNull] string id) where T : class
        {
            var model = await set.FindAsync(id);
            if (model != null)
            {
                set.Remove(model);
            }
            return model;
        }

        /// <summary>
        /// 通过条件标记第一条记录为删除
        /// </summary>
        /// <param name="set"></param>
        /// <param name="where"></param> 
        /// <returns></returns>
        public static async Task<T> RemoveAsync<T>(this DbSet<T> set, [NotNull] Expression<Func<T, bool>> where) where T : class
        {
            var model = await set.Where(where).FirstOrDefaultAsync();
            set.Remove(model);
            return model;
        }


        /// <summary>
        /// 通过条件标记记录为删除
        /// </summary>
        /// <param name="set"></param>
        /// <param name="id"></param> 
        /// <returns></returns>
        public static Task<T[]> RemoveRangeAsync<T>(this DbSet<T> set, IEnumerable<string> id) where T : class, IStringIdable
        {
            var where = System.Linq.Expressions.Predicate.CreateOrEqual<T, string>(item => item.Id, id);
            return set.RemoveRangeAsync(where);
        }

        /// <summary>
        /// 通过条件标记记录为删除
        /// </summary>
        /// <param name="set"></param>
        /// <param name="where"></param> 
        /// <returns></returns>
        public static async Task<T[]> RemoveRangeAsync<T>(this DbSet<T> set, [NotNull] Expression<Func<T, bool>> where) where T : class
        {
            var model = await set.Where(where).ToArrayAsync();
            set.RemoveRange(model);
            return model;
        }
    }
}
