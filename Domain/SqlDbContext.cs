using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Domain
{
    /// <summary>
    /// 表示数据库上下文
    /// </summary>
    public class SqlDbContext : DbContext
    {
        /// <summary>
        /// 菜单
        /// </summary>
        public DbSet<Menu> Menu { get; set; }

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
