using Microsoft.EntityFrameworkCore;
using VideoApi.Data;
using VideoApi.DTOs;
using VideoApi.Helpers;
using VideoApi.Models;

namespace VideoApi.Services;

/// <summary>
/// Service implementation providing operations for videos.  Handles
/// persistence, file storage and thumbnail generation.
/// </summary>
public class VideoService : IVideoService
{
    private readonly AppDbContext _context;
    private readonly ICategoryService _categoryService;
    private readonly IWebHostEnvironment _env;

    public VideoService(AppDbContext context, ICategoryService categoryService, IWebHostEnvironment env)
    {
        _context = context;
        _categoryService = categoryService;
        _env = env;
    }

    public async Task<List<Video>> GetAllAsync()
    {
        return await _context.Videos
            .Include(v => v.VideoCategories)
                .ThenInclude(vc => vc.Category)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Video> UploadAsync(VideoUploadDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
        {
            throw new ArgumentException("No file provided");
        }
        // Validate file type
        var allowedExtensions = new[] { ".mp4", ".avi", ".mov" };
        var extension = Path.GetExtension(dto.File.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Unsupported file type. Only MP4, AVI and MOV are allowed.");
        }
        // Validate file size
        if (dto.File.Length > 100 * 1024 * 1024)
        {
            throw new InvalidOperationException("Maximum allowed file size is 100MB.");
        }
        // Ensure upload directories exist
        var uploadsRoot = Path.Combine(_env.ContentRootPath, "UploadedVideos");
        var thumbsRoot = Path.Combine(uploadsRoot, "thumbnails");
        Directory.CreateDirectory(uploadsRoot);
        Directory.CreateDirectory(thumbsRoot);
        // Generate a unique filename to avoid collisions
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsRoot, fileName);
        // Save the uploaded file to disk
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(fileStream);
        }
        // Prepare the video entity
        var video = new Video
        {
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? string.Empty,
            FileName = fileName,
            FilePath = Path.Combine("UploadedVideos", fileName)
        };
        // Handle categories: create or retrieve existing ones
        foreach (var categoryName in dto.Categories.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var category = await _categoryService.GetOrCreateAsync(categoryName);
            video.VideoCategories.Add(new VideoCategory { Video = video, Category = category });
        }
        // Generate a thumbnail for the video.  Use a separate unique filename for the thumbnail.
        var thumbFileName = $"{Guid.NewGuid()}.jpg";
        var thumbFullPath = Path.Combine(thumbsRoot, thumbFileName);
        await ThumbnailGenerator.GenerateThumbnailAsync(filePath, thumbFullPath);
        video.ThumbnailFileName = thumbFileName;
        video.ThumbnailPath = Path.Combine("UploadedVideos", "thumbnails", thumbFileName);
        // Persist to the database
        _context.Videos.Add(video);
        await _context.SaveChangesAsync();
        return video;
    }

    public async Task<Stream?> GetVideoStreamAsync(int videoId)
    {
        var video = await _context.Videos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == videoId);
        if (video == null)
        {
            return null;
        }
        var fullPath = Path.Combine(_env.ContentRootPath, video.FilePath);
        if (!File.Exists(fullPath))
        {
            return null;
        }
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public async Task<Stream?> GetThumbnailStreamAsync(int videoId)
    {
        var video = await _context.Videos.AsNoTracking().FirstOrDefaultAsync(v => v.Id == videoId);
        if (video == null)
        {
            return null;
        }
        var fullPath = Path.Combine(_env.ContentRootPath, video.ThumbnailPath);
        if (!File.Exists(fullPath))
        {
            return null;
        }
        return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}