namespace Bibliotheca.Models.ViewModels
{
	public class PaginationViewModel
	{
		public int TotalPages { get; set; }
		public int PageCount { get; set; }
		public int PageSize { get; set; }

		public string Title { get; set; }
		public string Author { get; set; }
		public ICollection<Category> SelectedCategories { get; set; }
		public List<string> LoanStatus { get; set; }
		public DateOnly? LoanDate { get; set; }
		public string Email { get; set; }
	}
}
