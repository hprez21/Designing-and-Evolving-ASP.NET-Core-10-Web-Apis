namespace Globomantics.API.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    public double? AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> Tags { get; set; } = [];

}
