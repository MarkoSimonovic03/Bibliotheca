namespace Bibliotheca.Models.ViewModels
{
	public class BookCreateViewModel
	{
		public Book Book { get; set; }
		public IEnumerable<Category> AllCategories { get; set; }
	}
}
