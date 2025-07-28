using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using VideoApi.Data;
using VideoApi.DTOs;
using VideoApi.Models;
using VideoApi.Services;
using Xunit;

namespace VideoApi.Tests
{
    /// <summary>
    /// Unit tests for the VideoService.  These tests verify that invalid uploads
    /// throw expected exceptions and that a valid upload results in persisted
    /// entities and on‑disk files.
    /// </summary>
    public class VideoServiceTests
    {
        private DbContextOptions<AppDbContext> CreateOptions() => new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        /// <summary>
        /// Minimal IWebHostEnvironment implementation for tests.  Only the
        /// ContentRootPath is used by the service to determine where to write
        /// uploaded files and thumbnails.
        /// </summary>
        private class FakeEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Development;
            public string ApplicationName { get; set; } = "VideoApi.Tests";
            public string WebRootPath { get; set; } = string.Empty;
            public string ContentRootPath { get; set; } = string.Empty;
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }

        [Fact]
        public async Task UploadAsync_ShouldThrow_WhenNoFileProvided()
        {
            var options = CreateOptions();
            using var context = new AppDbContext(options);
            var categoryService = new CategoryService(context);
            var env = new FakeEnvironment { ContentRootPath = Path.GetTempPath() };
            var videoService = new VideoService(context, categoryService, env);
            var dto = new VideoUploadDto
            {
                Title = "Test",
                Description = "",
                Categories = new List<string>(),
                File = null!
            };
            await Assert.ThrowsAsync<ArgumentException>(() => videoService.UploadAsync(dto));
        }

        [Fact]
        public async Task UploadAsync_ShouldThrow_WhenFileTypeNotAllowed()
        {
            var options = CreateOptions();
            using var context = new AppDbContext(options);
            var categoryService = new CategoryService(context);
            var env = new FakeEnvironment { ContentRootPath = Path.GetTempPath() };
            var videoService = new VideoService(context, categoryService, env);
            // Create a dummy file with .txt extension
            using var stream = new MemoryStream(new byte[10]);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.txt");
            var dto = new VideoUploadDto
            {
                Title = "Invalid",
                Description = "",
                Categories = new List<string>(),
                File = file
            };
            await Assert.ThrowsAsync<InvalidOperationException>(() => videoService.UploadAsync(dto));
        }

        [Fact]
        public async Task UploadAsync_ShouldThrow_WhenFileSizeExceeded()
        {
            var options = CreateOptions();
            using var context = new AppDbContext(options);
            var categoryService = new CategoryService(context);
            var env = new FakeEnvironment { ContentRootPath = Path.GetTempPath() };
            var videoService = new VideoService(context, categoryService, env);
            // Create a dummy file larger than 100MB
            long largeSize = 101 * 1024 * 1024;
            using var stream = new MemoryStream(new byte[1]);
            var formFile = new FakeLargeFormFile(stream, largeSize, "large.mp4");
            var dto = new VideoUploadDto
            {
                Title = "Too big",
                Description = "",
                Categories = new List<string>(),
                File = formFile
            };
            await Assert.ThrowsAsync<InvalidOperationException>(() => videoService.UploadAsync(dto));
        }

        [Fact]
        public async Task UploadAsync_ShouldPersistVideoAndCategories_WhenValidInput()
        {
            var options = CreateOptions();
            using var context = new AppDbContext(options);
            var categoryService = new CategoryService(context);
            // Use a unique temporary directory for this test
            string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            var env = new FakeEnvironment { ContentRootPath = tempDir };
            var videoService = new VideoService(context, categoryService, env);
            // Create a dummy MP4 file
            byte[] data = new byte[10];
            new Random().NextBytes(data);
            using var stream = new MemoryStream(data);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "video.mp4");
            var dto = new VideoUploadDto
            {
                Title = "MyVideo",
                Description = "A test video",
                Categories = new List<string> { "Cat1", "Cat2" },
                File = file
            };
            var video = await videoService.UploadAsync(dto);
            // Ensure video persisted and categories created
            Assert.Equal("MyVideo", video.Title);
            Assert.Equal(2, video.VideoCategories.Count);
            // Verify file and thumbnail exist on disk
            string uploadedFilePath = Path.Combine(tempDir, video.FilePath);
            Assert.True(File.Exists(uploadedFilePath));
            string thumbnailPath = Path.Combine(tempDir, video.ThumbnailPath);
            Assert.True(File.Exists(thumbnailPath));
            // Clean up
            Directory.Delete(tempDir, true);
        }

        /// <summary>
        /// A minimal fake implementation of IFormFile that reports a custom length but delegates
        /// stream reading to an underlying stream.  Useful for simulating oversized uploads.
        /// </summary>
        private class FakeLargeFormFile : IFormFile
        {
            private readonly Stream _baseStream;
            private readonly long _length;
            private readonly string _fileName;

            public FakeLargeFormFile(Stream baseStream, long length, string fileName)
            {
                _baseStream = baseStream;
                _length = length;
                _fileName = fileName;
            }

            public string ContentType { get; set; } = "video/mp4";
            public string ContentDisposition { get; set; } = string.Empty;
            public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
            public long Length => _length;
            public string Name { get; set; } = "file";
            public string FileName => _fileName;
            public void CopyTo(Stream target) => _baseStream.CopyTo(target);
            public Task CopyToAsync(Stream target, System.Threading.CancellationToken cancellationToken = default)
            {
                return _baseStream.CopyToAsync(target, cancellationToken);
            }
            public Stream OpenReadStream() => _baseStream;
        }
    }
}