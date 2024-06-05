using Microsoft.EntityFrameworkCore;

namespace Shop.Repositories
{
    public class StockRepo : IStockRepo
    {
        private readonly ApplicationDbContext _context;

        public StockRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> GetStockById(int itemId) => await _context.Stocks.FirstOrDefaultAsync(s => s.ItemId == itemId);

        public async Task ManageStock(StockDTO stockToManage)
        {
            var existingStock = await GetStockById(stockToManage.ItemId);
            if (existingStock is null)
            {
                var stock = new Stock { ItemId = stockToManage.ItemId, Quantity = stockToManage.Quantity };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "")
        {
            var stocks = await (from item in _context.Items
                                join stock in _context.Stocks
                                on item.Id equals stock.ItemId
                                into item_stock
                                from itemStock in item_stock.DefaultIfEmpty()
                                where string.IsNullOrWhiteSpace(sTerm) || item.Name.ToLower().Contains(sTerm.ToLower())
                                select new StockDisplayModel
                                {
                                    ItemId = item.Id,
                                    ItemName = item.Name,
                                    Quantity = itemStock == null ? 0 : itemStock.Quantity
                                }
                                ).ToListAsync();
            return stocks;
        }

    }

    public interface IStockRepo
    {
        Task<IEnumerable<StockDisplayModel>> GetStocks(string sTerm = "");
        Task<Stock?> GetStockById(int itemId);
        Task ManageStock(StockDTO stockToManage);
    }
}