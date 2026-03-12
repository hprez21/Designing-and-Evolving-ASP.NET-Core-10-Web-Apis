using Globomantics.API.Models;
using System.Collections.Concurrent;

namespace Globomantics.API.Data;

public static class InMemoryCatalogStore
{
    public static readonly ConcurrentDictionary<Guid, Category> Categories = new();
    public static readonly ConcurrentDictionary<Guid, Product> Products = new();

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
                CategoryId = electronics.Id,
                CreatedAt = new DateTime(2025, 1, 15, 10, 0, 0, DateTimeKind.Utc),
                AverageRating = 4.5,
                ReviewCount = 128
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000002"),
                Name = "Clean Code",
                Description = "A Handbook of Agile Software Craftsmanship by Robert C. Martin",
                Price = 34.99m,
                CategoryId = books.Id,
                CreatedAt = new DateTime(2025, 2, 20, 14, 30, 0, DateTimeKind.Utc),
                AverageRating = 4.3,
                ReviewCount = 32
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000003"),
                Name = "Cotton T-Shirt",
                Description = "100% organic cotton crew neck t-shirt",
                Price = 19.99m,
                CategoryId = clothing.Id,
                CreatedAt = new DateTime(2025, 3, 10, 9, 15, 0, DateTimeKind.Utc),
                AverageRating = null,
                ReviewCount = 0
            },
            new Product
            {
                Id = Guid.Parse("b2c3d4e5-0002-0002-0002-000000000004"),
                Name = null,
                Description = "A plain blank notebook for writing, sketching, or note-taking",
                Price = 4.99m,
                CategoryId = books.Id,
                CreatedAt = new DateTime(2025, 4, 1, 8, 0, 0, DateTimeKind.Utc),
                AverageRating = null,
                ReviewCount = 0
            }
        };

        foreach (var product in products)
        {
            Products[product.Id] = product;
        }
    }
}
