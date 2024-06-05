
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Consts;

namespace Shop.Controllers
{
    [Authorize(Roles = nameof(Roles.Moderator))]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepo _categoryRepo;

        public CategoryController(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryRepo.GetCategories();
            return View(categories);
        }

        public IActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            try
            {
                var categoryToAdd = new Category { Name = category.CategoryName, Id = category.Id };
                await _categoryRepo.AddCategory(categoryToAdd);
                TempData["successMessage"] = "Category added successfully";
                return RedirectToAction(nameof(AddCategory));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Category could not added!";
                return View(category);
            }

        }

        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _categoryRepo.GetCategoryById(id);
            if (category is null)
                throw new InvalidOperationException($"Category with id: {id} does not found");
            var categoryToUpdate = new CategoryDTO
            {
                Id = category.Id,
                CategoryName = category.Name
            };
            return View(categoryToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryToUpdate)
        {
            if (!ModelState.IsValid)
            {
                return View(categoryToUpdate);
            }
            try
            {
                var category = new Category { Name = categoryToUpdate.CategoryName, Id = categoryToUpdate.Id };
                await _categoryRepo.UpdateCategory(category);
                TempData["successMessage"] = "Category is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Category could not updated!";
                return View(categoryToUpdate);
            }

        }

        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepo.GetCategoryById(id);
            if (category is null)
                throw new InvalidOperationException($"Category with id: {id} does not found");
            await _categoryRepo.DeleteCategory(category);
            return RedirectToAction(nameof(Index));

        }

    }
}