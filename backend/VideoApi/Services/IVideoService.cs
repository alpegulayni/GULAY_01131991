using Microsoft.AspNetCore.Mvc;
using VideoApi.DTOs;
using VideoApi.Models;

namespace VideoApi.Services;

/// <summary>
/// Service interface for managing video operations.  Abstracts the underlying
/// database and file system interactions so that controllers remain thin and
/// focused on HTTP concerns.
/// </summary>
public interface IVideoService
{
    /// <summary>
    /// Returns all videos stored in the system, including category information.
    /// </summary>
    /// <returns>A list of videos.</returns>
    Task<List<Video>> GetAllAsync();

    /// <summary>
    /// Handles the upload of a new video.  Saves the video file to disk,
    /// generates a thumbnail, creates or reuses categories and persists
    /// everything to the database.
    /// </summary>
    /// <param name="dto">Data transfer object containing metadata and file.</param>
    /// <returns>The created Video entity.</returns>
    Task<Video> UploadAsync(VideoUploadDto dto);

    /// <summary>
    /// Retrieves a stream for the requested video.  The caller is responsible
    /// for disposing of the returned stream.
    /// </summary>
    /// <param name="videoId">Identifier of the video.</param>
    /// <returns>Stream of the video file.</returns>
    Task<Stream?> GetVideoStreamAsync(int videoId);

    /// <summary>
    /// Retrieves a stream for the requested video thumbnail.  The caller is
    /// responsible for disposing of the returned stream.
    /// </summary>
    /// <param name="videoId">Identifier of the video.</param>
    /// <returns>Stream of the thumbnail image.</returns>
    Task<Stream?> GetThumbnailStreamAsync(int videoId);
}