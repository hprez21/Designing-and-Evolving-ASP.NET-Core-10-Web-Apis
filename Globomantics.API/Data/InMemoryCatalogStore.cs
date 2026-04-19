using Globomantics.API.Models;
using System.Collections.Concurrent;

namespace Globomantics.API.Data;

public static class InMemoryCatalogStore
{
    public static readonly ConcurrentDictionary<Guid, Category> Categories = new();
    public static readonly ConcurrentDictionary<Guid, Product> Products = new();

    public static readonly ConcurrentDictionary<Guid, Review> Reviews = new();

    static InMemoryCatalogStore()
    {
        Seed();
    }

    public static void Seed()
    {
        var electronics = new Category
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001"),
            Name = "Electronics",
            Description = "Electronic devices and accessories"
        };

        var books = new Category
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002"),
            Name = "Books",
            Description = "Physical and digital books"
        };

        var clothing = new Category
        {
            Id = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003"),
            Name = "Clothing",
            Description = "Apparel and fashion items"
        };

        Categories[electronics.Id] = electronics;
        Categories[books.Id] = books;
        Categories[clothing.Id] = clothing;

        var products = new[]
        {
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000001"),
                Name = "Wireless Headphones",
                Description = "Noise-cancelling over-ear headphones",
                Price = 79.99m,
                Pricing = new Pricing
                {
                    BasePrice = 79.99m,
                    Currency = "USD",
                    DiscountPercentage = 10m
                },
                CategoryId = electronics.Id,
                CreatedAt = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                AverageRating = 4.5,
                ReviewCount = 128,
                Tags = ["wireless", "audio", "noise-cancelling"]
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000002"),
                Name = "Clean Code",
                Description = "A Handbook of Agile Software Craftsmanship by Robert C. Martin",
                Price = 34.99m,
                Pricing = new Pricing { BasePrice = 34.99m, Currency = "USD" },
                CategoryId = books.Id,
                CreatedAt = new DateTime(2025, 2, 20, 14, 30, 0, DateTimeKind.Utc),
                AverageRating = 4.8,
                ReviewCount = 256,
                Tags = ["programming", "best-practices"]
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000003"),
                Name = "Cotton T-Shirt",
                Description = "100% organic cotton crew neck t-shirt",
                Price = 19.99m,
                Pricing = new Pricing { BasePrice = 19.99m, Currency = "USD" },
                CategoryId = clothing.Id,
                CreatedAt = new DateTime(2025, 3, 10, 9, 15, 0, DateTimeKind.Utc),
                AverageRating = null,
                ReviewCount = 0,
                Tags = ["organic", "cotton", "basics"]
            }

        };

        foreach (var product in products)
        {
            Products[product.Id] = product;
        }

        var reviews = new[]
        {
            new Review
            {
                Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000001"),
                ProductId = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000001"),
                Author = "Alice",
                Rating = 5,
                Comment = "Best headphones I've ever owned. ANC is incredible.",
                CreatedAt = new DateTime(2025, 4, 1, 12, 0, 0, DateTimeKind.Utc)
            },
            new Review
            {
                Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000002"),
                ProductId = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000001"),
                Author = "Bob",
                Rating = 4,
                Comment = "Great sound quality but a bit heavy for long listening sessions.",
                CreatedAt = new DateTime(2025, 4, 15, 9, 30, 0, DateTimeKind.Utc)
            },
            new Review
            {
                Id = Guid.Parse("c3d4e5f6-0003-0003-0003-000000000003"),
                ProductId = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000002"),
                Author = "Charlie",
                Rating = 5,
                Comment = "Essential reading for any software developer.",
                CreatedAt = new DateTime(2025, 5, 10, 16, 45, 0, DateTimeKind.Utc)
            }
        };

        foreach (var review in reviews)
        {
            Reviews[review.Id] = review;
        }
    }


}