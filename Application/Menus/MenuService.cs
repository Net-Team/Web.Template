using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Menus
{
    /// <summary>
    /// 表示菜单应用服务
    /// </summary>
    public class MenuService : TransientApplicationService
    {
        private readonly SqlDbContext db;

        /// <summary>
        /// 菜单应用服务
        /// </summary>
        /// <param name="db"></param>
        public MenuService(SqlDbContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// 获取指定用户所有菜单
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public async Task<Menu[]> GetMenusAsync(string userId)
        {
            return await db.Menu.Where(item => item.UserId == userId).ToArrayAsync();
        }
    }
}
