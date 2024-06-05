using Microsoft.AspNetCore.Mvc;
using Shop.Models.DTO;
using System.Diagnostics;

namespace Shop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepo _homeRepo;

        public HomeController(ILogger<HomeController> logger, IHomeRepo homeRepo)
        {
            _homeRepo = homeRepo;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string sterm = "", int? catId = null)
        {
            IEnumerable<Item> items = await _homeRepo.GetItems(sterm, catId);
            IEnumerable<Category> categories = await _homeRepo.GetCategories();

            ItemDisplayModel itemDisplayModel = new ItemDisplayModel
            {
                Items = items,
                Categories = categories,
                catId = catId  // Set catId here
            };
            return View(itemDisplayModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
