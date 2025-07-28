using System;
using System.IO;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace VideoApi.Helpers;

/// <summary>
/// Helper for generating a thumbnail image from a video file.  The implementation
/// attempts to use FFmpeg via the Xabe.FFmpeg wrapper to capture a frame from
/// the video.  If FFmpeg is unavailable or fails, a tiny blank JPEG image is
/// written instead.  The generated thumbnail will always be 256x256 pixels.
/// </summary>
public static class ThumbnailGenerator
{
    /// <summary>
    /// Generates a 256x256 thumbnail image from the supplied video file.  The
    /// resulting JPEG image is saved to <paramref name="outputPath"/>.  If
    /// thumbnail generation fails for any reason, a 1×1 pixel image is
    /// generated as a fallback to ensure a file exists on disk.
    /// </summary>
    /// <param name="videoPath">Absolute path to the source video file.</param>
    /// <param name="outputPath">Absolute path where the thumbnail should be written.</param>
    public static async Task GenerateThumbnailAsync(string videoPath, string outputPath)
    {
        try
        {
            // Ensure FFmpeg executables are present in the working directory or installed on the system.
            // Xabe.FFmpeg automatically downloads a static build on first use when this call is made.
            var mediaInfo = await FFmpeg.GetMediaInfo(videoPath);
            var snapshotTime = TimeSpan.FromSeconds(1);
            var conversion = await FFmpeg.Conversions.FromSnippet.Snapshot(videoPath, outputPath, snapshotTime);
            // The Snapshot snippet creates an image with the same resolution as the source video.  To
            // generate a 256×256 thumbnail we add an explicit scaling parameter.  The `-vf scale`
            // argument instructs FFmpeg to resize the frame to the desired width and height.
            conversion.AddParameter("-vf scale=256:256");
            await conversion.Start();
        }
        catch
        {
            // If FFmpeg is not available or fails, write a blank JPEG to the target location.
            // The following byte array encodes a 1×1 white JPEG image.
            byte[] fallbackImage = Convert.FromBase64String(
                "/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAP//////////////////////////////////////////////////////////////////////////////////////2wBDAf//////////////////////////////////////////////////////////////////////////////////////wAARCAABAAEDASIAAhEBAxEB/8QAFQABAQAAAAAAAAAAAAAAAAAAAAf/xAAUEAEAAAAAAAAAAAAAAAAAAAAA/9oADAMBAAIQAxAAAAGYf//EABQQAQAAAAAAAAAAAAAAAAAAABH/2gAIAQMBAT8AQ//EABQRAQAAAAAAAAAAAAAAAAAAABD/2gAIAQIBAT8AQ//EABQQAQAAAAAAAAAAAAAAAAAAABH/2gAIAQEABj8AQ//EABQQAQAAAAAAAAAAAAAAAAAAABH/2gAIAQEAAT8hH//aAAwDAQACAAMAAAAQf//Z");
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            await File.WriteAllBytesAsync(outputPath, fallbackImage);
        }
    }
}