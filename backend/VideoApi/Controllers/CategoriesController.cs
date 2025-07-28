using Microsoft.AspNetCore.Mvc;
using VideoApi.DTOs;
using VideoApi.Services;

namespace VideoApi.Controllers;

/// <summary>
/// API controller for working with video categories.  Exposes endpoints to
/// retrieve all categories or create new ones.  Category names are kept
/// unique by the underlying service.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> Get()
    {
        var categories = await _categoryService.GetAllAsync();
        var dtos = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name }).ToList();
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest(new { error = "Name is required" });
        }
        var category = await _categoryService.GetOrCreateAsync(dto.Name);
        return Ok(new CategoryDto { Id = category.Id, Name = category.Name });
    }
}