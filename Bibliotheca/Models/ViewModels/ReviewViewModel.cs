namespace Bibliotheca.Models.ViewModels
{
	public class ReviewViewModel
	{
		public int BookId { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public int Rating { get; set; }
		public string Text { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
