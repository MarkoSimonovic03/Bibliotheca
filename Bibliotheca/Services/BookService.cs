using Bibliotheca.Interfaces;
using Bibliotheca.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;

namespace Bibliotheca.Services
{
	public class BookService : IBookService
	{
		private readonly BibliothecaContext _context;
		public BookService(BibliothecaContext context)
		{
			_context = context;
		}

		public IEnumerable<Book> GetAllBooks()
		{
			return _context.Books.Include(b => b.Categories).Where(b => !b.IsDeleted).ToList();
		}

		public IEnumerable<Book> GetAllFilteredBooks(string title, string author, List<string> categories)
		{
			var books = _context.Books
				.Include(b => b.Categories)
				.Where(b => !b.IsDeleted)
				.AsQueryable();

			if (!string.IsNullOrEmpty(title))
			{
				books = books.Where(b => b.Title.ToUpper().Contains(title.Trim().ToUpper()));
			}

			if (!string.IsNullOrEmpty(author))
			{
				books = books.Where(b => b.Author.ToUpper().Contains(author.Trim().ToUpper()));
			}

			if (categories != null && categories.Any())
			{
				books = books.Where(b => categories.All(c => b.Categories.Any(bookCategory => bookCategory.Name == c)));
			}

			return books.ToList();
		}

		public Book GetBookWithReviewsAndUsersById(int id)
		{
			var book = _context.Books
				.Include(b => b.Categories)
				.Include(b => b.Reviews)
					.ThenInclude(r => r.User)
				.FirstOrDefault(b => b.Id == id && !b.IsDeleted);

			if (book != null)
			{
				book.Reviews = book.Reviews.OrderByDescending(r => r.CreatedAt).ToList();
			}

			return book;
		}

		public Book GetBookById(int id)
		{
			var book = _context.Books
				.Include(b => b.Categories)
				.FirstOrDefault(b => b.Id == id && !b.IsDeleted);

			return book;
		}


		public void AddBook(Book book)
		{
			_context.Books.Add(book);
		}

		public void UpdateBook(Book book)
		{
			_context.Books.Update(book);
		}

		public void DeleteBook(int id)
		{
			var book = _context.Books.Find(id);
			if (book != null)
			{
				book.IsDeleted = true;
				_context.Books.Update(book);
			}
		}
	}
}
