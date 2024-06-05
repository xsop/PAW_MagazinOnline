using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Consts;

namespace Shop.Controllers
{
    [Authorize(Roles = nameof(Roles.Moderator))]
    public class StockController : Controller
    {
        private readonly IStockRepo _stockRepo;

        public StockController(IStockRepo stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<IActionResult> Index(string sTerm="")
        {
            var stocks = await _stockRepo.GetStocks(sTerm);
            return View(stocks);
        }

        public async Task<IActionResult> ManageStock(int itemId)
        {
            var existingStock = await _stockRepo.GetStockById(itemId);
            var stock = new StockDTO
            {
                ItemId = itemId,
                Quantity = existingStock != null
            ? existingStock.Quantity : 0
            };
            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
                return View(stock);
            try
            {
                await _stockRepo.ManageStock(stock);
                TempData["successMessage"] = "Stock updated successfully.";
            }
            catch (Exception)
            {
                TempData["errorMessage"] = "Stock update error.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
