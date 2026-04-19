namespace Globomantics.API.DTOs.V1
{
    public class ProductResponseV1
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Obsolete("Use Pricing property instead. Will be removed in v2.")]
        public decimal Price { get; set; }
        public PricingResponse? Pricing { get; set; }
        public Guid CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<string> Tags { get; set; } = [];
        public string Status { get; set; } = "Active";
    }
}
