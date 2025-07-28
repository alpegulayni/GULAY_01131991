namespace VideoApi.Models;

/// <summary>
/// Represents a single uploaded video.  The database stores references to the 
/// physical file as well as a generated thumbnail image.  A video may belong 
/// to zero or more categories via the VideoCategories join table.
/// </summary>
public class Video
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ThumbnailFileName { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public ICollection<VideoCategory> VideoCategories { get; set; } = new List<VideoCategory>();
}