using Globomantics.API.Data;
using Globomantics.API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var categories = InMemoryCatalogStore.Categories.Values
            .Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            })
            .ToList();

        return Ok(categories);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        if (!InMemoryCatalogStore.Categories.TryGetValue(id, out var category))
            return NotFound();

        return Ok(new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        });
    }
}
