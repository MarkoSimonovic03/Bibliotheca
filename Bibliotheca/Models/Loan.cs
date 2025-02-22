using Bibliotheca.Models.enums;

namespace Bibliotheca.Models
{
    public class Loan
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User? User { get; set; }
        public int BookId { get; set; }
        public Book? Book { get; set; }
        public DateOnly LoanDate { get; set; }
        public DateOnly DueDate { get; set; }
        public LoanStatus LoanStatus { get; set; }
    }
}
