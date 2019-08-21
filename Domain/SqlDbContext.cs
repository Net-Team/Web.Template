using Microsoft.EntityFrameworkCore;

namespace Domain
{
    /// <summary>
    /// 表示数据库上下文
    /// </summary>
    public class SqlDbContext : DbContext
    { 
        /// <summary>
        /// 数据库上下文
        /// </summary>
        /// <param name="options"></param>
        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
