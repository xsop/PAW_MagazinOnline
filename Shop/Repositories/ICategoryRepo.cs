﻿using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;

namespace Shop.Repositories;

public interface ICategoryRepo
{
    Task AddCategory(Category category);
    Task UpdateCategory(Category category);
    Task<Category?> GetCategoryById(int id);
    Task DeleteCategory(Category category);
    Task<IEnumerable<Category>> GetCategories();
}
public class CategoryRepo : ICategoryRepo
{
    private readonly ApplicationDbContext _context;
    public CategoryRepo(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddCategory(Category category)
    {
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateCategory(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        return await _context.Categories.ToListAsync();
    }


}