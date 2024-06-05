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

        public async Task<IEnumerable<Item>> GetItems(string sTerm = "", int? catId = null)
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Item> items = await (from i in _db.Items
                                             join c in _db.Categories on i.CategoryId equals c.Id
                                             join s in _db.Stocks on i.Id equals s.ItemId into stocks
                                             from s in stocks.DefaultIfEmpty()
                                             where (i.Name.ToLower().Contains(sTerm) || i.Description.ToLower().Contains(sTerm)) && (catId == null || i.CategoryId == catId)
                                             select new Item
                                             {
                                                 Id = i.Id,
                                                 Name = i.Name,
                                                 Description = i.Description,
                                                 Price = i.Price,
                                                 Image = i.Image,
                                                 CategoryId = i.CategoryId,
                                                 Category = c,
                                                 CategoryName = c.Name,
                                                 Quantity =s==null?0:s.Quantity
                                             }).ToListAsync();
            return items;
        }
    }
}
