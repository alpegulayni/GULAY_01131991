using Microsoft.AspNetCore.Mvc;
using VideoApi.DTOs;
using VideoApi.Models;
using VideoApi.Services;

namespace VideoApi.Controllers;

/// <summary>
/// API controller responsible for interacting with videos.  Provides
/// endpoints to list videos, upload new ones and stream both the full
/// video file and its generated thumbnail.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VideosController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideosController(IVideoService videoService)
    {
        _videoService = videoService;
    }

    /// <summary>
    /// Returns all available videos along with metadata and links to stream
    /// both the video and its thumbnail.  The links are generated based on
    /// the current request's scheme and host.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoDto>>> Get()
    {
        var videos = await _videoService.GetAllAsync();
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var dtos = videos.Select(v => new VideoDto
        {
            Id = v.Id,
            Title = v.Title,
            Description = v.Description,
            Categories = v.VideoCategories.Select(vc => vc.Category.Name).ToList(),
            ThumbnailUrl = $"{baseUrl}/api/videos/{v.Id}/thumbnail",
            VideoUrl = $"{baseUrl}/api/videos/{v.Id}/stream"
        }).ToList();
        return Ok(dtos);
    }

    /// <summary>
    /// Uploads a new video.  The request must be submitted as multipart/form-data
    /// with exactly one file and appropriate metadata.  Upon success the
    /// created video DTO is returned.
    /// </summary>
    [HttpPost("upload")]
    [RequestSizeLimit(100 * 1024 * 1024)] // enforce maximum body size at the action level
    public async Task<ActionResult<VideoDto>> Upload([FromForm] VideoUploadDto uploadDto)
    {
        try
        {
            var video = await _videoService.UploadAsync(uploadDto);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var dto = new VideoDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                Categories = video.VideoCategories.Select(vc => vc.Category.Name).ToList(),
                ThumbnailUrl = $"{baseUrl}/api/videos/{video.Id}/thumbnail",
                VideoUrl = $"{baseUrl}/api/videos/{video.Id}/stream"
            };
            return CreatedAtAction(nameof(Get), new { id = video.Id }, dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Streams the full video file to the client.  Returns 404 if the video
    /// cannot be found on disk.
    /// </summary>
    [HttpGet("{id}/stream")]
    public async Task<IActionResult> Stream(int id)
    {
        var stream = await _videoService.GetVideoStreamAsync(id);
        if (stream == null)
        {
            return NotFound();
        }
        // Determine content type based on file extension
        var allVideos = await _videoService.GetAllAsync();
        var video = allVideos.FirstOrDefault(v => v.Id == id);
        string extension = video?.FileName != null ? Path.GetExtension(video.FileName).ToLower() : string.Empty;
        string contentType = extension switch
        {
            ".mp4" => "video/mp4",
            ".avi" => "video/x-msvideo",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };
        return File(stream, contentType, enableRangeProcessing: true);
    }

    /// <summary>
    /// Streams the generated thumbnail image to the client.  Returns 404 if
    /// the thumbnail is missing.
    /// </summary>
    [HttpGet("{id}/thumbnail")]
    public async Task<IActionResult> Thumbnail(int id)
    {
        var stream = await _videoService.GetThumbnailStreamAsync(id);
        if (stream == null)
        {
            return NotFound();
        }
        return File(stream, "image/jpeg");
    }
}