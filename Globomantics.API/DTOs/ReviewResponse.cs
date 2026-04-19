namespace Globomantics.API.DTOs
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Author { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
