using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace VideoApi.DTOs;

/// <summary>
/// DTO used when a client uploads a new video.  Includes metadata and the
/// actual file.  Validation attributes enforce required fields.
/// </summary>
public class VideoUploadDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(160)]
    public string? Description { get; set; }
;
    /// <summary>
    /// Categories provided by the client.  Duplicate names are ignored and new
    /// categories will be created server‑side.
    /// </summary>
    public List<string> Categories { get; set; } = new();

    /// <summary>
    /// Uploaded video file.  Only one file is accepted per request.
    /// </summary>
    [Required]
    public IFormFile? File { get; set; }
;
}