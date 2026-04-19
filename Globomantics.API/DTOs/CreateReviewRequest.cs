using System.ComponentModel.DataAnnotations;

namespace Globomantics.API.DTOs
{
    public class CreateReviewRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }

}
