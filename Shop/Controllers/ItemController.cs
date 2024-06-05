
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shop.Consts;

namespace Shop.Controllers;

[Authorize(Roles = nameof(Roles.Moderator))]
public class ItemController : Controller
{
    private readonly IItemRepo _itemRepo;
    private readonly ICategoryRepo _categoryRepo;

    public ItemController(IItemRepo itemRepo, ICategoryRepo categoryRepo)
    {
        _itemRepo = itemRepo;
        _categoryRepo = categoryRepo;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemRepo.GetItems();
        return View(items);
    }

    public async Task<IActionResult> AddItem()
    {
        var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
        {
            Text = category.Name,
            Value = category.Id.ToString(),
        });
        ItemDTO itemToAdd = new() { CategoryList = categorySelectList };
        return View(itemToAdd);
    }

    [HttpPost]
    public async Task<IActionResult> AddItem(ItemDTO itemToAdd)
    {
        var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
        {
            Text = category.Name,
            Value = category.Id.ToString(),
        });
        itemToAdd.CategoryList = categorySelectList;

        if (!ModelState.IsValid)
            return View(itemToAdd);

        try
        {
            
            // manual mapping of ItemDTO -> Item
            Item item = new()
            {
                Id = itemToAdd.Id,
                Name = itemToAdd.ItemName,
                Description = itemToAdd.Description,
                Image = itemToAdd.Image,
                CategoryId = itemToAdd.CategoryId,
                Price = itemToAdd.Price
            };
            await _itemRepo.AddItem(item);
            TempData["successMessage"] = "Item is added successfully";
            return RedirectToAction(nameof(AddItem));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToAdd);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToAdd);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(itemToAdd);
        }
    }

    public async Task<IActionResult> UpdateItem(int id)
    {
        var item = await _itemRepo.GetItemById(id);
        if (item == null)
        {
            TempData["errorMessage"] = $"Item with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }
        var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
        {
            Text = category.Name,
            Value = category.Id.ToString(),
            Selected = category.Id == item.CategoryId
        });
        ItemDTO itemToUpdate = new()
        {
            CategoryList = categorySelectList,
            ItemName = item.Name,
            Description = item.Description,
            CategoryId = item.CategoryId,
            Price = item.Price,
            Image = item.Image
        };
        return View(itemToUpdate);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItem(ItemDTO itemToUpdate)
    {
        var categorySelectList = (await _categoryRepo.GetCategories()).Select(category => new SelectListItem
        {
            Text = category.Name,
            Value = category.Id.ToString(),
            Selected = category.Id == itemToUpdate.CategoryId
        });
        itemToUpdate.CategoryList = categorySelectList;

        if (!ModelState.IsValid)
            return View(itemToUpdate);

        try
        {
            string oldImage = "";
            
            // manual mapping of ItemDTO -> Item
            Item item = new()
            {
                Id = itemToUpdate.Id,
                Name = itemToUpdate.ItemName,
                Description = itemToUpdate.Description,
                CategoryId = itemToUpdate.CategoryId,
                Price = itemToUpdate.Price,
                Image = itemToUpdate.Image
            };
            await _itemRepo.UpdateItem(item);
            // if image is updated, then delete it from the folder too
            
            TempData["successMessage"] = "Item is updated successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToUpdate);
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
            return View(itemToUpdate);
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on saving data";
            return View(itemToUpdate);
        }
    }

    public async Task<IActionResult> DeleteItem(int id)
    {
        try
        {
            var item = await _itemRepo.GetItemById(id);
            if (item == null)
            {
                TempData["errorMessage"] = $"Item with the id: {id} does not found";
            }
            else
            {
                await _itemRepo.DeleteItem(item);
            }
        }
        catch (InvalidOperationException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (FileNotFoundException ex)
        {
            TempData["errorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            TempData["errorMessage"] = "Error on deleting the data";
        }
        return RedirectToAction(nameof(Index));
    }
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _itemRepo.GetItemById(id);
        if (item == null)
        {
            TempData["errorMessage"] = $"Item with the id: {id} does not found";
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

}