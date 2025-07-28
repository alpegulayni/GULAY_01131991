using VideoApi.Models;

namespace VideoApi.Services;

/// <summary>
/// Service interface for managing categories.  Abstracts away database
/// interactions and encapsulates the logic for deduplicating category names.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Returns all categories in the system sorted alphabetically.
    /// </summary>
    Task<List<Category>> GetAllAsync();

    /// <summary>
    /// Retrieves an existing category by name or creates a new one if none
    /// exists.  Category names are compared case‑insensitively.
    /// </summary>
    /// <param name="name">The category name.</param>
    Task<Category> GetOrCreateAsync(string name);
}