namespace Globomantics.API.Models
{
    public class Pricing
    {
        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal? DiscountPercentage { get; set; }

        public decimal EffectivePrice => DiscountPercentage.HasValue
            ? BasePrice * (1 - DiscountPercentage.Value / 100m)
            : BasePrice;
    }
}
