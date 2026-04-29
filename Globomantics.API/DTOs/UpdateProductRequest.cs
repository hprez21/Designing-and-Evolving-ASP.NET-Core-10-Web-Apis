using System.ComponentModel.DataAnnotations;

namespace Globomantics.API.DTOs;

public class UpdateProductRequest
{
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}
