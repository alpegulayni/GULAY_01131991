namespace VideoApi.DTOs;

/// <summary>
/// DTO used to return video information to clients.  Includes a collection of
/// category names as well as URLs pointing to the video stream and
/// thumbnail image.  The URLs are generated based on the configured API
/// routes in the VideosController.
/// </summary>
public class VideoDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
}