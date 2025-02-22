using Bibliotheca.Interfaces;

namespace Bibliotheca.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BibliothecaContext _context;
        public UnitOfWork(BibliothecaContext context)
        {
            _context = context;
            BookService = new BookService(context);
            LoanService = new LoanService(context);
            ReviewService = new ReviewService(context);
			CategoryService = new CategoryService(context);
        }

        public IBookService BookService { get; private set; }
        public ILoanService LoanService { get; private set; }
        public IReviewService ReviewService { get; private set; }
        public ICategoryService CategoryService { get; private set; }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
