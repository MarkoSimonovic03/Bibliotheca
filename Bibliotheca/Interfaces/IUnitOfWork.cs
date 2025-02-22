using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Bibliotheca.Interfaces
{
    public interface IUnitOfWork
    {
        void SaveChanges();
        IBookService BookService { get; }
        ILoanService LoanService { get; }
        IReviewService ReviewService { get; }
        ICategoryService CategoryService { get; }
	}
}
