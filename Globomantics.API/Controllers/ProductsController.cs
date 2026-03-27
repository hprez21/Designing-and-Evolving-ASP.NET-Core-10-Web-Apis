using ApiEvolutionLab.DTOs;
using Globomantics.API.Data;
using Globomantics.API.DTOs;
using Globomantics.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Controllers;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var products = InMemoryCatalogStore.Products.Values
            .Select(MapToResponse)
            .ToList();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        if (!InMemoryCatalogStore.Products.TryGetValue(id, out var product))
            return NotFound();

        return Ok(MapToResponse(product));
    }

    [HttpPut("{id:guid}/price")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    public IActionResult UpdatePrice(Guid id, [FromQuery] decimal price)
    {
        if (!InMemoryCatalogStore.Products.TryGetValue(id, out var existing))
            return NotFound();

        if (price <= 0)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid price",
                Detail = "Price must be greater than zero.",
                Status = StatusCodes.Status400BadRequest
            });

        existing.Price = price;

        return NoContent();
    }


    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Create([FromBody] CreateProductRequest request)
    {
        if (!InMemoryCatalogStore.Categories.ContainsKey(request.CategoryId))
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid category",
                Detail = $"Category with id '{request.CategoryId}' does not exist.",
                Status = StatusCodes.Status400BadRequest
            });

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        InMemoryCatalogStore.Products[product.Id] = product;

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            MapToResponse(product));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        if (!InMemoryCatalogStore.Products.TryGetValue(id, out var existing))
            return NotFound();

        if (!InMemoryCatalogStore.Categories.ContainsKey(request.CategoryId))
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid category",
                Detail = $"Category with id '{request.CategoryId}' does not exist.",
                Status = StatusCodes.Status400BadRequest
            });

        existing.Name = request.Name;
        existing.Description = request.Description;
        existing.Price = request.Price;
        existing.CategoryId = request.CategoryId;

        return Ok(MapToResponse(existing));
    }

    //// TODO: remove before next release — added during sprint 4 for the data team
    //// Returns a flat CSV-friendly representation of a product for bulk exports.
    //// Never formally reviewed; response shape has NOT been agreed with API consumers.
    //[HttpGet("{id:guid}/export")]
    //public IActionResult ExportProduct(Guid id)
    //{
    //    if (!InMemoryCatalogStore.Products.TryGetValue(id, out var product))
    //        return NotFound();

    //    InMemoryCatalogStore.Categories.TryGetValue(product.CategoryId, out var category);

    //    var export = new
    //    {
    //        product_id    = product.Id.ToString(),
    //        product_name  = product.Name,
    //        unit_price    = product.Price,
    //        category      = category?.Name ?? "Uncategorized",
    //        is_active     = product.IsActive,
    //        created_utc   = product.CreatedAt.ToString("o")
    //    };

    //    return Ok(export);
    //}

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!InMemoryCatalogStore.Products.TryRemove(id, out _))
            return NotFound();

        return NoContent();
    }

    private static ProductResponse MapToResponse(Product product)
    {
        InMemoryCatalogStore.Categories.TryGetValue(product.CategoryId, out var category);

        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name,
            CreatedAt = product.CreatedAt
        };
    }
}
