using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.Data;
using VideoApi.Services;
using VideoApi.Models;
using Xunit;

namespace VideoApi.Tests
{
    /// <summary>
    /// Unit tests for the CategoryService.  These tests use the in-memory
    /// database provider to verify that categories are created and retrieved
    /// correctly and that duplicate names do not result in multiple entries.
    /// </summary>
    public class CategoryServiceTests
    {
        private DbContextOptions<AppDbContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                // Use a unique in-memory database for each test to prevent cross‑test contamination
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetOrCreateAsync_ShouldReturnSameCategory_ForDuplicateNameIgnoringCase()
        {
            var options = CreateOptions();
            using (var context = new AppDbContext(options))
            {
                var service = new CategoryService(context);
                // First call should create the category
                var first = await service.GetOrCreateAsync("Test");
                // Second call with different casing should return the same entity
                var second = await service.GetOrCreateAsync("test");

                Assert.Equal(first.Id, second.Id);
                Assert.Equal(1, context.Categories.Count());
            }
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnCategoriesInAlphabeticalOrder()
        {
            var options = CreateOptions();
            using (var context = new AppDbContext(options))
            {
                context.Categories.AddRange(
                    new Category { Name = "Banana" },
                    new Category { Name = "Apple" },
                    new Category { Name = "Cherry" });
                await context.SaveChangesAsync();

                var service = new CategoryService(context);
                var categories = await service.GetAllAsync();

                Assert.Equal(new[] { "Apple", "Banana", "Cherry" }, categories.Select(c => c.Name).ToArray());
            }
        }
    }
}