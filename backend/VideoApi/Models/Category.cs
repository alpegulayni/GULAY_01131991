namespace VideoApi.Models;

/// <summary>
/// Represents a video category.  Category names are unique to prevent duplicates.
/// </summary>
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<VideoCategory> VideoCategories { get; set; } = new List<VideoCategory>();
}