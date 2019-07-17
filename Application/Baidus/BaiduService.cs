using Domain;

namespace Application.Baidus
{
    public class BaiduService : TransientApplicationService
    {
        private readonly SqlDbContext db;

        public BaiduService(SqlDbContext db)
        {
            this.db = db;
        }

        public int Sum(int x, int y)
        {
            return x + y;
        }
    }
}
