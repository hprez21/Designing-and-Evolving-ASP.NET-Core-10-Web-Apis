using System.ComponentModel.DataAnnotations;

namespace ApiEvolutionLab.DTOs;

public class CreateProductRequest
{    
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, 999999.99)]
    public decimal Price { get; set; }

    [Required]
    public Guid CategoryId { get; set; }
}