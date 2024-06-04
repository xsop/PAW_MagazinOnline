using Microsoft.EntityFrameworkCore;

namespace Shop.Repositories
{
    public class HomeRepo : IHomeRepo
    {
        private readonly ApplicationDbContext _db;
        public HomeRepo(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<IEnumerable<Item>> GetItems(string sTerm="", int catId=0)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Item> items = await(from i in _db.Items
                         join c in _db.Categories on i.CategoryId equals c.Id
                         where (i.Name.ToLower().Contains(sTerm) || i.Description.ToLower().Contains(sTerm)) && (catId == 0 || i.CategoryId == catId)
                         select new Item
                         {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            Price = i.Price,
                            Image = i.Image,
                            CategoryId = i.CategoryId,
                            Category = c,
                            CategoryName = c.Name
                         }).ToListAsync();
            return items;
        }
    }
}
