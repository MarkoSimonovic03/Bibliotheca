using Bibliotheca.Interfaces;
using Bibliotheca.Models;
using Bibliotheca.Models.enums;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Services
{
    public class LoanService : ILoanService
    {
        private readonly BibliothecaContext _context;
        public LoanService(BibliothecaContext context)
        {
            _context = context;
        }

        public IEnumerable<Loan> GetAllLoans()
        {
            return _context.Loans.ToList();
        }

        public Loan GetLoanById(int id)
        {
            return _context.Loans.Find(id);
        }



        public IEnumerable<Loan> GetLoansByUserId(string userId, List<string> loanStatus, DateOnly? loanDate)
        {
            var loans = _context.Loans
                .Where(l => l.UserId == userId)
                .Include(l => l.Book)
                .OrderByDescending(l => l.Id)
                .AsQueryable();


			if (loanStatus != null && loanStatus.Any())
            {
                loans = loans.Where(l => loanStatus.Contains(l.LoanStatus.ToString()));
            }

            if (loanDate.HasValue)
            {
                loans = loans.Where(l => l.LoanDate == loanDate.Value);
            }

			loans.OrderBy(l => l.Id).ToList();

			foreach (var loan in loans)
			{

				if (loan.LoanStatus == LoanStatus.InProgress && loan.DueDate < DateOnly.FromDateTime(DateTime.Now))
				{
					loan.LoanStatus = LoanStatus.Overdue;
				}
			}

			_context.SaveChanges();

			return loans;
        }

        public void AddLoan(Loan loan)
        {
            _context.Loans.Add(loan);
        }
        public void UpdateLoan(Loan loan)
        {
            _context.Loans.Update(loan);
        }

        public void DeleteLoan(int id)
        {
            var loan = _context.Loans.Find(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }
        }

        //public IEnumerable<Loan> GetAllLoansWithUsersAndBooks()
        //{
        //    var loans = _context.Loans.Include(l => l.User).Include(l => l.Book).ToList();

        //    foreach (var loan in loans)
        //    {

        //        if (loan.LoanStatus == LoanStatus.InProgress && loan.DueDate < DateOnly.FromDateTime(DateTime.Now))
        //        {
        //            loan.LoanStatus = LoanStatus.Overdue;
        //        }
        //    }

        //    _context.SaveChanges();

        //    return loans;
        //}

        public IEnumerable<Loan> GetAllLoansWithUsersAndBooks(string email, List<string> loanStatus, DateOnly? loanDate)
        {
			var loans = _context.Loans
                .Include(l => l.User)
				.Include(l => l.Book)
				.OrderByDescending(l => l.Id)
				.AsQueryable();

            if (!string.IsNullOrEmpty(email))
            {
                loans = loans.Where(l => l.User.Email.ToUpper().Contains(email.Trim().ToUpper()));
            }

            if (loanStatus != null && loanStatus.Any())
            {
                loans = loans.Where(l => loanStatus.Contains(l.LoanStatus.ToString()));
            }

            if (loanDate.HasValue)
            {
                loans = loans.Where(l => l.LoanDate == loanDate.Value);
            }

            loans.ToList();


			foreach (var loan in loans)
			{

				if (loan.LoanStatus == LoanStatus.InProgress && loan.DueDate < DateOnly.FromDateTime(DateTime.Now))
				{
					loan.LoanStatus = LoanStatus.Overdue;
				}
			}

			_context.SaveChanges();

			return loans;
        }

        public void ReturnLoan(int id)
        {
            var loan = _context.Loans.Find(id);
            if (loan != null) {
			    loan.LoanStatus = LoanStatus.Returned;

			    var book = _context.Books.Find(loan.BookId);
			    book.AvailableQuantity += 1;
            }
		}
    }
}
