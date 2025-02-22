using Bibliotheca.Models;

namespace Bibliotheca.Interfaces
{
    public interface ILoanService
    {
        IEnumerable<Loan> GetAllLoans();
        Loan GetLoanById(int id);
        void AddLoan(Loan loan);
        void UpdateLoan(Loan loan);
        void DeleteLoan(int id);
        IEnumerable<Loan> GetLoansByUserId(string userId, List<string> loanStatus, DateOnly? loanDate);
        IEnumerable<Loan> GetAllLoansWithUsersAndBooks(string email, List<string> loanStatus, DateOnly? loanDate);
        public void ReturnLoan(int id);

	}
}
