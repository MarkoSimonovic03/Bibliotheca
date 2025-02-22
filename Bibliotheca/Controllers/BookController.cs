using Bibliotheca.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Bibliotheca.Models;
using Bibliotheca.Models.enums;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Models.ViewModels;
using System.Drawing.Printing;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Net;
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
			var categories = _unitOfWork.CategoryService.GetAllCategories();

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
					PageSize = pageSize
				},
				Categories = categories
			};

		
			return View(modelView);
		}

		public async Task<IActionResult> Details(int id)
		{
			var book = _unitOfWork.BookService.GetBookById(id);

            var averageRating = book.Reviews.Any() ? _unitOfWork.ReviewService.GetAverageRatingByBookId(book.Id) : 0;

            var viewModel = new BookDetailViewModel()
            {
                Book = book,
                AverageRating = averageRating
            };

            return View(viewModel);
		}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
		{
			var categories = _unitOfWork.CategoryService.GetAllCategories();
			return View(new Book { Categories = categories, YearOfPublication = DateTime.Now.Year, NumberOfPages = 200, AvailableQuantity = 10});
		}

        [Authorize(Roles = "Admin")]
        [HttpPost]
		public async Task<IActionResult> Create(Book book, IFormFile file, IEnumerable<int> categories)
		{
			if (ModelState.IsValid)
			{
                if (file != null)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\books");
                    path = $"{path}\\{book.Title}-{book.Author}.jpg";

                    FileStream stream = new FileStream(path, FileMode.Create);
                    file.CopyTo(stream);

                    book.ImageUrl = $"{book.Title}-{book.Author}.jpg"; ;
                }

                var selectedCategories = _unitOfWork.CategoryService.GetCategoriesByIds(categories);

				book.Categories = selectedCategories;

                _unitOfWork.BookService.AddBook(book);
				_unitOfWork.SaveChanges();

				return RedirectToAction("Index");
            }
			return View(book);
		}

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
		{
			var book = _unitOfWork.BookService.GetBookById(id);
			var allCategories = _unitOfWork.CategoryService.GetAllCategories();

			var viewModel = new BookEditViewModel()
			{
				Book = book,
				AllCategories = allCategories
			};


			return View(viewModel);
		}

        [Authorize(Roles = "Admin")]
        [HttpPost]
		public async Task<IActionResult> Edit(Book book, IFormFile? file, ICollection<int> categories)
		{
			var thisBook = _unitOfWork.BookService.GetBookById(book.Id);
			if (ModelState.IsValid)
			{
				if (file != null)
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\books");
					path = $"{path}\\{book.Title}-{book.Author}.jpg";

					FileStream stream = new FileStream(path, FileMode.Create);
					file.CopyTo(stream);

					book.ImageUrl = $"{book.Title}-{book.Author}.jpg"; ;
				}
				else
				{
					book.ImageUrl = thisBook.ImageUrl;
				}

				var selectedCategories = _unitOfWork.CategoryService.GetCategoriesByIds(categories);

				thisBook.Categories.Clear();
				thisBook.Categories = thisBook.Categories.ToList();
				thisBook.Categories.AddRange(selectedCategories);


				thisBook.Title = book.Title;
				thisBook.Author = book.Author;
				thisBook.Summary = book.Summary;
				thisBook.Publisher = book.Publisher;
				thisBook.YearOfPublication = book.YearOfPublication;
				thisBook.NumberOfPages = book.NumberOfPages;
				thisBook.AvailableQuantity = book.AvailableQuantity;


				_unitOfWork.BookService.UpdateBook(thisBook);
				_unitOfWork.SaveChanges();

				return RedirectToAction("Details", new { id = book.Id });
			}

			book = _unitOfWork.BookService.GetBookById(book.Id);
			var allCategories = _unitOfWork.CategoryService.GetAllCategories();

			var viewModel = new BookEditViewModel()
			{
				Book = book,
				AllCategories = allCategories
			};


			return View(viewModel);
		}

        [Authorize(Roles = "Admin")]
        [HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			_unitOfWork.BookService.DeleteBook(id);
			_unitOfWork.SaveChanges();

			var books = _unitOfWork.BookService.GetAllBooks();

			//TempData["Message"] = "Book successfully deleted!";
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> Review(int id)
		{
			var book = _unitOfWork.BookService.GetBookById(id);

			if (book == null) return NotFound();

			return View(new ReviewViewModel { BookId = book.Id, Title = book.Title, Author = book.Author });
		}

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

				book = _unitOfWork.BookService.GetBookById(book.Id);

				var averageRating = book.Reviews.Any() ? _unitOfWork.ReviewService.GetAverageRatingByBookId(book.Id) : 0;

				var viewModel = new BookDetailViewModel()
				{
					Book = book,
					AverageRating = averageRating
				};

				return RedirectToAction("Details", new { id = book.Id });
            }

            return View(new ReviewViewModel { BookId = book.Id, Title = book.Title, Author = book.Author });
        }
    }
}
