namespace Globomantics.API.DTOs
{
    public class PricingResponse
    {
        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal? DiscountPercentage { get; set; }
        public decimal EffectivePrice { get; set; }
    }
}
