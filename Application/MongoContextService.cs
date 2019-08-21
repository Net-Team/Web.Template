using Core;
using Domain;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Application
{
    /// <summary>
    /// 表示基于Mongodb的抽象服务
    /// </summary>
    public abstract class MongoContextService<T> : ScopedApplicationService where T : class, IMongoEntity
    {
        /// <summary>
        /// 获取db名
        /// </summary>
        public string DbName => "Talkback";

        /// <summary>
        /// 获取mongodb
        /// </summary>
        protected MongoDbContext MongoDb { get; }

        /// <summary>
        /// 基于Mongodb的抽象服务
        /// </summary>
        /// <param name="mongoDb"></param>
        public MongoContextService(MongoDbContext mongoDb)
        {
            this.MongoDb = mongoDb;
        }

        /// <summary>
        /// 保存日志
        /// </summary>
        /// <param name="log">日志</param> 
        /// <returns></returns>
        public virtual async Task AddAsync(T log)
        {
            var dbName = this.GetDbName(DateTime.Now);
            await this.MongoDb.Set<T>(dbName).AddAsync(log);
        }

        /// <summary>
        /// 条件删除第一条
        /// </summary>
        /// <param name="where">日志</param> 
        /// <returns></returns>
        public virtual async Task<T> RemoveOneAsync(Expression<Func<T, bool>> where)
        {
            var dbName = this.GetDbName(DateTime.Now);
            return await this.MongoDb.Set<T>(dbName).RemoveOneAsync(where);
        }

        /// <summary>
        /// 条件删除
        /// </summary>
        /// <param name="where">条件</param> 
        /// <returns></returns>
        public virtual async Task<long> RemoveAsync(Expression<Func<T, bool>> where)
        {
            var dbName = this.GetDbName(DateTime.Now);
            return await this.MongoDb.Set<T>(dbName).RemoveAsync(where);
        }

        /// <summary>
        /// 返回分页查询
        /// </summary>
        /// <param name="month">获取哪个月</param>
        /// <param name="where">条件</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <returns></returns>
        public virtual Task<Page<T>> GetPageAsync(DateTime month, Expression<Func<T, bool>> where, int pageIndex, int pageSize)
        {
            var dbName = this.GetDbName(month);
            return this.MongoDb.Set<T>(dbName).ToPageAsync(where, item => item.CreateTime, false, pageIndex, pageSize);
        }

        /// <summary>
        /// 返回dbName
        /// </summary>
        /// <param name="month">月分</param>
        /// <returns></returns>
        protected virtual string GetDbName(DateTime month)
        {
            return $"{this.DbName}_{DateTime.Now.ToString("yyMM")}";
        }
    }
}
