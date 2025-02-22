namespace Bibliotheca.Models.ViewModels
{
	public class BookIndexViewModel
	{
		public IEnumerable<Book> Books { get; set; }
		public PaginationViewModel Pagination { get; set; }
		public IEnumerable<Category> Categories { get; set; }
	}
}
