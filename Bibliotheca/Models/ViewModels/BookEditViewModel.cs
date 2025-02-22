namespace Bibliotheca.Models.ViewModels
{
	public class BookEditViewModel
	{
		public Book Book { get; set; }
		public IEnumerable<Category> AllCategories { get; set; }
	}
}
