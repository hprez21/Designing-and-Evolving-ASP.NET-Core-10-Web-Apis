using ApiEvolutionLab.DTOs;
using Globomantics.API.Data;
using Globomantics.API.DTOs;
using Globomantics.API.DTOs.V1;
using Globomantics.API.Mappers;
using Globomantics.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Globomantics.API.Controllers.V1;

[ApiController]
[Route("v1/products")]
[Produces("application/json")]
[Tags("Products")]
public class Productsv1Controller : ControllerBase
{
    [HttpGet]
    [EndpointGroupName("v1")]
    [ProducesResponseType(typeof(IEnumerable<ProductResponseV1>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var products = InMemoryCatalogStore.Products.Values
            .Select(ProductMapper.ToV1Response)
            .ToList();

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [EndpointGroupName("v1")]
    [ProducesResponseType(typeof(ProductResponseV1), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(Guid id)
    {
        if (!InMemoryCatalogStore.Products.TryGetValue(id, out var product))
            return NotFound();

        return Ok(ProductMapper.ToV1Response(product));
    }

    [HttpPost]
    [EndpointGroupName("v1")]
    [ProducesResponseType(typeof(ProductResponseV1), StatusCodes.Status201Created)]
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
            Pricing = new Pricing { BasePrice = request.Price },
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            Tags = request.Tags ?? []
        };

        InMemoryCatalogStore.Products[product.Id] = product;

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            ProductMapper.ToV1Response(product));
    }

    [HttpPut("{id:guid}")]
    [EndpointGroupName("v1")]
    [ProducesResponseType(typeof(ProductResponseV1), StatusCodes.Status200OK)]
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
        existing.Pricing = new Pricing { BasePrice = request.Price };
        existing.CategoryId = request.CategoryId;

        return Ok(ProductMapper.ToV1Response(existing));
    }

    [HttpDelete("{id:guid}")]
    [EndpointGroupName("v1")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Guid id)
    {
        if (!InMemoryCatalogStore.Products.TryRemove(id, out _))
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    [EndpointGroupName("v1")]
    [Consumes("application/merge-patch+json")]
    [ProducesResponseType(typeof(ProductResponseV1), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Patch(Guid id, [FromBody] JsonElement patchDocument)
    {
        if (!InMemoryCatalogStore.Products.TryGetValue(id, out var existing))
            return NotFound();

        if (patchDocument.TryGetProperty("name", out var nameEl))
        {
            var name = nameEl.GetString();
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid name",
                    Detail = "Product name cannot be empty.",
                    Status = StatusCodes.Status400BadRequest
                });
            existing.Name = name;
        }

        if (patchDocument.TryGetProperty("description", out var descEl))
        {
            existing.Description = descEl.ValueKind == JsonValueKind.Null
                ? null
                : descEl.GetString();
        }

        if (patchDocument.TryGetProperty("price", out var priceEl))
        {
            var price = priceEl.GetDecimal();
            if (price < 0.01m || price > 999999.99m)
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid price",
                    Detail = "Price must be between 0.01 and 999999.99.",
                    Status = StatusCodes.Status400BadRequest
                });
            existing.Price = price;
            existing.Pricing = new Pricing { BasePrice = price };
        }

        if (patchDocument.TryGetProperty("categoryId", out var catEl))
        {
            var categoryId = catEl.GetGuid();
            if (!InMemoryCatalogStore.Categories.ContainsKey(categoryId))
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid category",
                    Detail = $"Category with id '{categoryId}' does not exist.",
                    Status = StatusCodes.Status400BadRequest
                });
            existing.CategoryId = categoryId;
        }

        if (patchDocument.TryGetProperty("tags", out var tagsEl))
        {
            existing.Tags = tagsEl.ValueKind == JsonValueKind.Null
                ? []
                : tagsEl.EnumerateArray().Select(e => e.GetString()!).ToList();
        }

        return Ok(ProductMapper.ToV1Response(existing));
    }
}
