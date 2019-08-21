using Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 表示Mongo的集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MongoDbSet<T> where T : class, IStringIdable
    {
        /// <summary>
        /// 获取原始集合对象
        /// </summary>
        public IMongoCollection<T> Collection { get; }

        /// <summary>
        /// Mongo的集合
        /// </summary>
        /// <param name="collection">集合</param>
        public MongoDbSet([NotNull]IMongoCollection<T> collection)
        {
            this.Collection = collection;
        }

        /// <summary>
        /// 转换为IQueryable
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> AsQueryable()
        {
            return this.Collection.AsQueryable(new AggregateOptions { AllowDiskUse = true });
        }

        /// <summary>
        /// 清除所有记录
        /// </summary>
        public void Clear()
        {
            this.Collection.Database.DropCollection(typeof(T).Name);
        }

        /// <summary>
        /// 清除所有记录
        /// </summary>
        /// <returns></returns>
        public Task ClearAsync()
        {
            return this.Collection.Database.DropCollectionAsync(typeof(T).Name);
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Find([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Collection.Find(where).FirstOrDefault();
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<T> FindAsync([NotNull]Expression<Func<T, bool>> where)
        {
            var cursor = await this.Collection.FindAsync(where);
            return await cursor.FirstOrDefaultAsync();
        }

        /// <summary>
        /// 查找多项
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public List<T> FindMany([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Collection.Find(where).ToList();
        }

        /// <summary>
        /// 查找多项
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public async Task<List<T>> FindManyAsync([NotNull]Expression<Func<T, bool>> where)
        {
            var cursor = await this.Collection.FindAsync(where);
            return await cursor.ToListAsync();
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Any([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Collection
                .Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Match(where)
                .Any();
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public Task<bool> AnyAsync([NotNull]Expression<Func<T, bool>> where)
        {
            return this.Collection
                .Aggregate(new AggregateOptions { AllowDiskUse = true })
                .Match(where)
                .AnyAsync();
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="model">模型</param>
        public void Add([NotNull]T model)
        {
            this.Collection.InsertOne(model);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="model">模型</param>
        /// <returns></returns>
        public Task AddAsync([NotNull]T model)
        {
            return this.Collection.InsertOneAsync(model);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="models">模型</param>
        public void AddRange([NotNull]IEnumerable<T> models)
        {
            this.Collection.InsertMany(models);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="models">模型</param>
        /// <returns ></returns>
        public Task AddRangeAsync([NotNull]IEnumerable<T> models)
        {
            return this.Collection.InsertManyAsync(models);
        }

        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="updater">更新</param>
        /// <returns></returns>
        public int Update([NotNull]Expression<Func<T, bool>> where, [NotNull] UpdateDefinition<T> updater)
        {
            return (int)this.Collection.UpdateMany(where, updater).ModifiedCount;
        }

        /// <summary>
        /// 条件更新
        /// </summary>
        /// <param name="where">条件</param>
        /// <param name="updater">更新</param>
        /// <returns></returns>
        public async Task<int> UpdateAsync([NotNull]Expression<Func<T, bool>> where, [NotNull]UpdateDefinition<T> updater)
        {
            var result = await this.Collection.UpdateManyAsync(where, updater);
            return (int)result.ModifiedCount;
        }


        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public async Task<long> RemoveAsync([NotNull]Expression<Func<T, bool>> where)
        {
            var result = await this.Collection.DeleteManyAsync(where);
            return result.DeletedCount;
        }

        /// <summary>
        /// 条件删除第一条
        /// </summary>
        /// <param name="where">条件</param>
        /// <returns></returns>
        public async Task<T> RemoveOneAsync([NotNull]Expression<Func<T, bool>> where)
        {
            return await this.Collection.FindOneAndDeleteAsync(where);
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="where">条件</param>
        /// <param name="orderBy">排序</param>
        /// <param name="asc">是否升序</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public async Task<Page<T>> ToPageAsync<TKey>([NotNull]Expression<Func<T, bool>> where, [NotNull]Expression<Func<T, TKey>> orderBy, bool asc, int pageIndex, int pageSize)
        {
            int total = (int)await this.Collection.CountDocumentsAsync(where);
            var inc = total % pageSize > 0 ? 0 : -1;
            var maxPageIndex = (int)Math.Floor((double)total / pageSize) + inc;
            pageIndex = Math.Max(0, Math.Min(pageIndex, maxPageIndex));

            var sort = CastSortField(orderBy);
            var query = this.Collection
               .Aggregate(new AggregateOptions { AllowDiskUse = true })
               .Match(where);

            if (asc == true)
            {
                query = query.SortBy(sort);
            }
            else
            {
                query = query.SortByDescending(sort);
            }

            var datas = await query
                .Skip(pageIndex * pageSize)
                .Limit(pageSize)
                .ToListAsync();

            return new Page<T>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = total,
                DataArray = datas.ToArray()
            };
        }

        /// <summary>
        /// 键选择类型转换
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="keySelector">键选择</param>
        /// <returns></returns>
        private static Expression<Func<T, object>> CastSortField<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var body = Expression.Convert(keySelector.Body, typeof(object));
            return Expression.Lambda<Func<T, object>>(body, keySelector.Parameters);
        }
    }
}
