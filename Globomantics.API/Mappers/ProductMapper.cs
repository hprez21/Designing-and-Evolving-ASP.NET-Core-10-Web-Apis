using Globomantics.API.Data;
using Globomantics.API.DTOs;
using Globomantics.API.DTOs.V1;
using Globomantics.API.DTOs.V2;
using Globomantics.API.Models;

namespace Globomantics.API.Mappers
{
    public static class ProductMapper
    {
        public static ProductResponseV1 ToV1Response(Product product)
        {
            InMemoryCatalogStore.Categories.TryGetValue(product.CategoryId, out var category);

#pragma warning disable CS0618
            return new ProductResponseV1
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Pricing.EffectivePrice,
                Pricing = ToPricingResponse(product.Pricing),
                CategoryId = product.CategoryId,
                CategoryName = category?.Name,
                CreatedAt = product.CreatedAt,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                Tags = product.Tags,
                Status = product.Status
            };
#pragma warning restore CS0618
        }

        public static ProductResponseV2 ToV2Response(Product product)
        {
            InMemoryCatalogStore.Categories.TryGetValue(product.CategoryId, out var category);

            return new ProductResponseV2
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Pricing = ToPricingResponse(product.Pricing),
                CategoryId = product.CategoryId,
                CategoryName = category?.Name,
                CreatedAt = product.CreatedAt,
                AverageRating = product.AverageRating,
                ReviewCount = product.ReviewCount,
                Tags = product.Tags,
                Status = product.Status
            };
        }

        private static PricingResponse ToPricingResponse(Pricing pricing) => new()
        {
            BasePrice = pricing.BasePrice,
            Currency = pricing.Currency,
            DiscountPercentage = pricing.DiscountPercentage,
            EffectivePrice = pricing.EffectivePrice
        };
    }

}
