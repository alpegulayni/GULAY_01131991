using Microsoft.EntityFrameworkCore;
using VideoApi.Data;
using VideoApi.Models;

namespace VideoApi.Services;

/// <summary>
/// Implementation of <see cref="ICategoryService"/> backed by Entity Framework Core.
/// Provides helper methods for retrieving and creating categories.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly AppDbContext _context;

    public CategoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category> GetOrCreateAsync(string name)
    {
        var normalized = name.Trim();
        // Check if a category with the given name (case‑insensitive) already exists
        var existing = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToLower() == normalized.ToLower());
        if (existing != null)
        {
            return existing;
        }
        var category = new Category { Name = normalized };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }
}