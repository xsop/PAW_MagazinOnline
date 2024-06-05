using Microsoft.EntityFrameworkCore;

namespace Shop.Repositories
{
    public interface IItemRepo
    {
        Task AddItem(Item item);
        Task DeleteItem(Item item);
        Task<Item?> GetItemById(int id);
        Task<IEnumerable<Item>> GetItems();
        Task UpdateItem(Item item);
    }

    public class ItemRepo : IItemRepo
    {
        private readonly ApplicationDbContext _context;
        public ItemRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddItem(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItem(Item item)
        {
            _context.Items.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteItem(Item item)
        {
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<Item?> GetItemById(int id) => await _context.Items.FindAsync(id);

        public async Task<IEnumerable<Item>> GetItems() => await _context.Items.Include(a => a.Category).ToListAsync();
    }
}