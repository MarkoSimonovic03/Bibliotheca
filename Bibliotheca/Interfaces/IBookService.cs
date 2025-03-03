using Bibliotheca.Models;

namespace Bibliotheca.Interfaces
{
    public interface IBookService
    {
        IEnumerable<Book> GetAllBooks();
        IEnumerable<Book> GetAllFilteredBooks(string title, string author, List<string> categories);
        Book GetBookWithReviewsAndUsersById(int id);
		Book GetBookById(int id);
        void AddBook(Book book);
        void UpdateBook(Book book);
        void DeleteBook(int id);
    }
}
