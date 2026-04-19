using Globomantics.API.Data;
using Globomantics.API.DTOs;
using Globomantics.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Globomantics.API.Controllers
{
    [ApiController]
    [Route("products/{productId:guid}/reviews")]
    [Produces("application/json")]
    public class ProductReviewsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReviews(Guid productId)
        {
            if (!InMemoryCatalogStore.Products.ContainsKey(productId))
                return NotFound(new ProblemDetails
                {
                    Title = "Product not found",
                    Detail = $"Product with id '{productId}' does not exist.",
                    Status = StatusCodes.Status404NotFound
                });

            var reviews = InMemoryCatalogStore.Reviews.Values
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(MapToResponse)
                .ToList();

            return Ok(reviews);
        }

        [HttpGet("{reviewId:guid}")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetReview(Guid productId, Guid reviewId)
        {
            if (!InMemoryCatalogStore.Products.ContainsKey(productId))
                return NotFound(new ProblemDetails
                {
                    Title = "Product not found",
                    Detail = $"Product with id '{productId}' does not exist.",
                    Status = StatusCodes.Status404NotFound
                });

            if (!InMemoryCatalogStore.Reviews.TryGetValue(reviewId, out var review)
                || review.ProductId != productId)
                return NotFound();

            return Ok(MapToResponse(review));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CreateReview(Guid productId, [FromBody] CreateReviewRequest request)
        {
            if (!InMemoryCatalogStore.Products.ContainsKey(productId))
                return NotFound(new ProblemDetails
                {
                    Title = "Product not found",
                    Detail = $"Product with id '{productId}' does not exist.",
                    Status = StatusCodes.Status404NotFound
                });

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = productId,
                Author = request.Author,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            InMemoryCatalogStore.Reviews[review.Id] = review;

            return CreatedAtAction(
                nameof(GetReview),
                new { productId, reviewId = review.Id },
                MapToResponse(review));
        }

        private static ReviewResponse MapToResponse(Review review) => new()
        {
            Id = review.Id,
            ProductId = review.ProductId,
            Author = review.Author,
            Rating = review.Rating,
            Comment = review.Comment,
            CreatedAt = review.CreatedAt
        };
    }
}
