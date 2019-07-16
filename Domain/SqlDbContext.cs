using Microsoft.EntityFrameworkCore;
using System;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
