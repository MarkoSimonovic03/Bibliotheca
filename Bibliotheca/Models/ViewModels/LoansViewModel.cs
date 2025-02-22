namespace Bibliotheca.Models.ViewModels
{
	public class LoansViewModel
	{
		public IEnumerable<Loan> Loans { get; set; }
		public PaginationViewModel Pagination { get; set; }
	}
}
