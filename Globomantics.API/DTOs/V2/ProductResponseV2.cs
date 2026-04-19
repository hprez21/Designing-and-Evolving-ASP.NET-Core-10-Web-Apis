namespace Globomantics.API.DTOs.V2
{
    public class ProductResponseV2
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public PricingResponse Pricing { get; set; } = new();
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Tags { get; set; } = [];
        public string Status { get; set; } = "Active";
    }
}
