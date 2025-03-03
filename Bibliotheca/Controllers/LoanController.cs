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
			if (book == null) return NotFound();

			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

			if (book.AvailableQuantity <= 0) return BadRequest("The book is out of stock.");

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

			TempData["Message"] = "The book has been successfully loaned!";

			return RedirectToAction("UserLoans");
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AllLoans(string email, List<string> loanStatus, DateOnly? loanDate, int pageCount = 1, int pageSize = 5)
		{
			var loans = _unitOfWork.LoanService.GetAllLoansWithUsersAndBooks(email, loanStatus, loanDate);

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
					PageSize = pageSize,
					LoanStatus = loanStatus,
					LoanDate = loanDate,
					Email = email
				}
			};

			return View(modelView);
		}

		public IActionResult UserLoans(List<string> loanStatus, DateOnly? loanDate, int pageCount = 1, int pageSize = 5)
		{
			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

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
					PageSize = pageSize,
					LoanStatus = loanStatus,
					LoanDate = loanDate
				}
			};

			return View(modelView);
		}


		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> ReturnAdminLoan(int id)
		{
			_unitOfWork.LoanService.ReturnLoan(id);
			_unitOfWork.SaveChanges();

			TempData["Message"] = "The book has been successfully returned!";

			return RedirectToAction("AllLoans");
		}

		[HttpPost]
		public async Task<IActionResult> ReturnUserLoan(int id)
		{
			var userId = User.Identity.IsAuthenticated ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;

			var loan = _unitOfWork.LoanService.GetLoanById(id);

			if (loan.UserId == userId)
			{
				_unitOfWork.LoanService.ReturnLoan(id);
				_unitOfWork.SaveChanges();
				TempData["Message"] = "The book has been successfully returned!";
			}

			return RedirectToAction("UserLoans");
		}
	}
}
