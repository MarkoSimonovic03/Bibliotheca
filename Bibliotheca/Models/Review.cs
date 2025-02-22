using System.ComponentModel.DataAnnotations;

namespace Bibliotheca.Models
{
	public class Review
	{
		[Required]
		public int Id { get; set; }
		[Required]
		public int BookId { get; set; }
		public string? UserId { get; set; }
		public User? User { get; set; }
		[Required]
		[Range(0, 5)]
		public int Rating { get; set; }
		[Required]
		public string Text { get; set; }
		[Required]
		public DateTime CreatedAt { get; set; }
	}
}
