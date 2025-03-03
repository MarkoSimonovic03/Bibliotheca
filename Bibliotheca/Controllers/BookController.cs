using Bibliotheca.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Bibliotheca.Models;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Models.ViewModels;
using System.Security.Claims;
using NuGet.Packaging;

namespace Bibliotheca.Controllers
{
	public class BookController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public BookController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IActionResult> Index(string title, string author, List<string> Categories, int pageCount = 1, int pageSize = 12)
		{
			var books = _unitOfWork.BookService.GetAllFilteredBooks(title, author, Categories);

			var totalPages = (int)Math.Ceiling((double)books.Count() / pageSize);
			pageCount = (pageCount < 1) ? 1 : (pageCount > totalPages ? totalPages : pageCount);
			books = books.Skip(pageSize * (pageCount - 1)).Take(pageSize);

			var modelView = new BookIndexViewModel()
			{
				Books = books,
				Pagination = new PaginationViewModel()
				{
					TotalPages = totalPages,
					PageCount = pageCount,
					PageSize = pageSize,
					Title = title,
					Author = author,
					SelectedCategories = _unitOfWork.CategoryService.GetCategoriesByNames(Categories)
				},
				Categories = _unitOfWork.CategoryService.GetAllCategories()
			};

			return View(modelView);
		}

		public async Task<IActionResult> Details(int id)
		{
			var book = _unitOfWork.BookService.GetBookWithReviewsAndUsersById(id);
			if (book == null) return NotFound();

			var viewModel = new BookDetailViewModel()
			{
				Book = book,
				AverageRating = _unitOfWork.ReviewService.GetAverageRatingByBookId(book.Id)
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create()
		{
			var book = new Book
			{
				YearOfPublication = DateTime.Now.Year,
				NumberOfPages = 200,
				AvailableQuantity = 10,
				Categories = new List<Category>()
			};

			var viewModel = new BookCreateViewModel()
			{
				Book = book,
				AllCategories = _unitOfWork.CategoryService.GetAllCategories()
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Create(Book book, IFormFile file, IEnumerable<int> categories)
		{
			book.Categories = _unitOfWork.CategoryService.GetCategoriesByIds(categories);

			if (ModelState.IsValid)
			{
				if (file != null)
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\books");
					path = $"{path}\\{book.Title}-{book.Author}.jpg";

					using (FileStream stream = new FileStream(path, FileMode.Create))
					{
						file.CopyTo(stream);
					}

					book.ImageUrl = $"{book.Title}-{book.Author}.jpg"; ;
				}

				_unitOfWork.BookService.AddBook(book);
				_unitOfWork.SaveChanges();

				TempData["Message"] = "The book has been successfully created!";

				return RedirectToAction("Details", new { id = book.Id });
			}

			var viewModel = new BookCreateViewModel()
			{
				Book = book,
				AllCategories = _unitOfWork.CategoryService.GetAllCategories()
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id)
		{
			var book = _unitOfWork.BookService.GetBookById(id);
			if (book == null) return NotFound();

			var viewModel = new BookCreateViewModel()
			{
				Book = book,
				AllCategories = _unitOfWork.CategoryService.GetAllCategories()
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Edit(Book book, IFormFile? file, ICollection<int> categories)
		{
			var thisBook = _unitOfWork.BookService.GetBookById(book.Id);
			if (thisBook == null) return NotFound();

			thisBook.Categories.Clear();
			thisBook.Categories = thisBook.Categories.ToList();
			thisBook.Categories.AddRange(_unitOfWork.CategoryService.GetCategoriesByIds(categories));

			if (ModelState.IsValid)
			{
				if (file != null)
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\books");
					path = $"{path}\\{book.Title}-{book.Author}.jpg";

					using (FileStream stream = new FileStream(path, FileMode.Create))
					{
						file.CopyTo(stream);
					}

					thisBook.ImageUrl = $"{book.Title}-{book.Author}.jpg"; ;
				}

				thisBook.Title = book.Title;
				thisBook.Author = book.Author;
				thisBook.Summary = book.Summary;
				thisBook.Publisher = book.Publisher;
				thisBook.YearOfPublication = book.YearOfPublication;
				thisBook.NumberOfPages = book.NumberOfPages;
				thisBook.AvailableQuantity = book.AvailableQuantity;

				_unitOfWork.BookService.UpdateBook(thisBook);
				_unitOfWork.SaveChanges();

				TempData["Message"] = "The changes have been successfully saved!";

				return RedirectToAction("Details", new { id = thisBook.Id });
			}

			var viewModel = new BookCreateViewModel()
			{
				Book = thisBook,
				AllCategories = _unitOfWork.CategoryService.GetAllCategories()
			};

			return View(viewModel);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			_unitOfWork.BookService.DeleteBook(id);
			_unitOfWork.SaveChanges();

			TempData["Message"] = "The book has been successfully deleted!";

			return RedirectToAction("Index");
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Review(Review review)
		{
			var book = _unitOfWork.BookService.GetBookById(review.BookId);
			if (book == null) return NotFound();

			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
			review.UserId = userId;

			if (ModelState.IsValid)
			{
				review.CreatedAt = DateTime.Now;
				_unitOfWork.ReviewService.AddReview(review);
				_unitOfWork.SaveChanges();

				TempData["Message"] = "Your review has been successfully submitted.";
			}

			return RedirectToAction("Details", new { id = book.Id });
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> DeleteReview(int reviewId, int bookId)
		{
			_unitOfWork.ReviewService.DeleteReview(reviewId);
			_unitOfWork.SaveChanges();

			TempData["Message"] = "The review has been successfully deleted.";

			return RedirectToAction("Details", new { id = bookId });
		}
	}
}