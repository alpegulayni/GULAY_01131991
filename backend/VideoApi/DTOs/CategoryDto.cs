namespace VideoApi.DTOs;

/// <summary>
/// Simple DTO for exposing category information to clients.  Includes an
/// identifier and the category's unique name.
/// </summary>
public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}