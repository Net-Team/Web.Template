using MongoDB.Driver;
using System.Diagnostics.CodeAnalysis;

namespace Domain
{
    /// <summary>
    /// 表示mongoDb上下文
    /// </summary>    
    public class MongoDbContext
    {
        /// <summary>
        /// 客户端
        /// </summary>
        private readonly MongoClient client;

        /// <summary>
        /// mongoDb上下文
        /// </summary>   
        /// <param name="connectionString">mongodb://[username:password@]host1[:port1][,host2[:port2],…[,hostN[:portN]]][/[database][?options]]</param>
        public MongoDbContext([NotNull]string connectionString)
        {
            this.client = new MongoClient(connectionString);
        }

        /// <summary>
        /// 获取T类型的集合操作
        /// </summary>
        /// <typeparam name="T">集合对象类型</typeparam>
        /// <param name="dbName">db名称</param>
        /// <returns></returns>
        public MongoDbSet<T> Set<T>([NotNull]string dbName) where T : class, IStringIdable
        {
            return this.Set<T>(dbName, typeof(T).Name);
        }

        /// <summary>
        /// 获取集合操作
        /// </summary>
        /// <typeparam name="T">集合对象类型</typeparam>
        /// <param name="dbName">db名称</param>
        /// <param name="collectionName">集合名称</param>
        /// <returns></returns>
        public MongoDbSet<T> Set<T>([NotNull]string dbName, [NotNull]string collectionName) where T : class, IStringIdable
        {
            var db = client.GetDatabase(dbName);
            var collection = db.GetCollection<T>(collectionName);
            return new MongoDbSet<T>(collection);
        }
    }
}
