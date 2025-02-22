using System.Drawing.Printing;
using System.Security.Claims;
using Bibliotheca.Interfaces;
using Bibliotheca.Models;
using Bibliotheca.Models.enums;
using Bibliotheca.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Reflection.Metadata.BlobBuilder;

namespace Bibliotheca.Controllers
{
	[Authorize]
	public class LoanController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public LoanController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpPost]
		public IActionResult Create(int id)
		{
			var book = _unitOfWork.BookService.GetBookById(id);

			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
			if (userId == null)
			{
				throw new Exception("User not authorized");
			}

			book.AvailableQuantity -= 1;

			var loan = new Loan()
			{
				BookId = book.Id,
				UserId = userId,
				LoanDate = DateOnly.FromDateTime(DateTime.Now),
				DueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
				LoanStatus = LoanStatus.InProgress
			};

			_unitOfWork.LoanService.AddLoan(loan);
			_unitOfWork.BookService.UpdateBook(book);
			_unitOfWork.SaveChanges();

			var vm = new LoanConfirmationViewModel()
			{
				Book = book,
				Loan = loan
			};

			return View("Confirmation", vm);
		}


		[Authorize(Roles = "Admin")]
		public IActionResult AllLoans(string username, List<string> loanStatus, DateOnly? loanDate, int pageCount = 1, int pageSize = 5)
		{
			var loans = _unitOfWork.LoanService.GetAllLoansWithUsersAndBooks(username, loanStatus, loanDate);

			var totalPages = (int)Math.Ceiling((double)loans.Count() / pageSize);
			pageCount = (pageCount < 1) ? 1 : (pageCount > totalPages ? totalPages : pageCount);
			loans = loans.Skip(pageSize * (pageCount - 1)).Take(pageSize);

			var modelView = new LoansViewModel()
			{
				Loans = loans,
				Pagination = new PaginationViewModel()
				{
					TotalPages = totalPages,
					PageCount = pageCount,
					PageSize = pageSize
				}
			};

			return View(modelView);
		}


		[Authorize]
		public IActionResult UserLoans(List<string> loanStatus, DateOnly? loanDate, int pageCount = 1, int pageSize = 5)
		{
			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
			if (userId == null)
			{
				throw new Exception("User not authorized");
			}

			var loans = _unitOfWork.LoanService.GetLoansByUserId(userId, loanStatus, loanDate);

			var totalPages = (int)Math.Ceiling((double)loans.Count() / pageSize);
			pageCount = (pageCount < 1) ? 1 : (pageCount > totalPages ? totalPages : pageCount);
			loans = loans.Skip(pageSize * (pageCount - 1)).Take(pageSize);

			var modelView = new LoansViewModel()
			{
				Loans = loans,
				Pagination = new PaginationViewModel()
				{
					TotalPages = totalPages,
					PageCount = pageCount,
					PageSize = pageSize
				}
			};

			return View(modelView);
		}

		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> ReturnAdminLoan(int id)
		{
			_unitOfWork.LoanService.ReturnLoan(id);
			_unitOfWork.SaveChanges();

			return RedirectToAction("AllLoans");
		}


		[Authorize]
		public async Task<IActionResult> ReturnUserLoan(int id)
		{
			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
			if (userId == null)
			{
				throw new Exception("User not authorized");
			}

			var loan = _unitOfWork.LoanService.GetLoanById(id);

			if(loan.UserId == userId)
			{
				_unitOfWork.LoanService.ReturnLoan(id);
				_unitOfWork.SaveChanges();
			}

			return RedirectToAction("UserLoans");
		}

		


		//[Authorize(Roles = "Admin")]
		//[HttpPost]
		//public async Task<IActionResult> ReturnAllLoans(int id)
		//{
		//	var loan = _unitOfWork.LoanService.GetLoanById(id);
		//	loan.LoanStatus = LoanStatus.Returned;

		//	var book = _unitOfWork.BookService.GetBookById(loan.BookId);
		//	book.AvailableQuantity += 1;

		//	_unitOfWork.LoanService.UpdateLoan(loan);
		//	_unitOfWork.BookService.UpdateBook(book);
		//	_unitOfWork.SaveChanges();

		//	var loans = _unitOfWork.LoanService.GetAllLoansWithUsersAndBooks();

		//	TempData["Message"] = "Book successfully returned!";
		//	return View("AllLoans", loans);
		//}




		//[HttpPost]
		//public async Task<IActionResult> ReturnUserLoans(int id)
		//{
		//	var loan = _unitOfWork.LoanService.GetLoanById(id);
		//	loan.LoanStatus = LoanStatus.Returned;

		//	var book = _unitOfWork.BookService.GetBookById(loan.BookId);
		//	book.AvailableQuantity += 1;

		//	_unitOfWork.LoanService.UpdateLoan(loan);
		//	_unitOfWork.BookService.UpdateBook(book);
		//	_unitOfWork.SaveChanges();

		//	var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
		//	if (userId == null)
		//	{
		//		throw new Exception("User not authorized");
		//	}

		//	var loans = _unitOfWork.LoanService.GetLoansByUserId(userId);

		//	TempData["Message"] = "Book successfully returned!";
		//	return View("UserLoans", loans);
		//}
	}
}
