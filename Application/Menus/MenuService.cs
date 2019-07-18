using Domain;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Menus
{
    public class MenuService : TransientApplicationService
    {
        private readonly SqlDbContext db;

        public MenuService(SqlDbContext db)
        {
            this.db = db;
        }

        public async Task<Menu[]> GetMenusAsync(string userId)
        {
            return await db.Menu.Where(item => item.UserId == userId).ToArrayAsync();
        }
    }
}
