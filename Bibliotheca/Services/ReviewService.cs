using Bibliotheca.Interfaces;
using Bibliotheca.Models;
using Microsoft.EntityFrameworkCore;

namespace Bibliotheca.Services
{
	public class ReviewService : IReviewService
	{
		private readonly BibliothecaContext _context;

		public ReviewService(BibliothecaContext context)
		{
			_context = context;
		}

		public void AddReview(Review review)
		{
			_context.Reviews.Add(review);
		}

		public void DeleteReview(int id)
		{
			var review = _context.Reviews.Find(id);

			if (review != null)
			{
				_context.Remove(review);
			}
		}

		public IEnumerable<Review> GetAllReviews()
		{
			return _context.Reviews.ToList();
		}

		public Review GetReviewById(int id)
		{
			return _context.Reviews.Find(id);
		}

		public IEnumerable<Review> GetReviewsByBookId(int bookId)
		{
			var reviews = _context.Reviews.Include(x => x.User)
			   .Where(r => r.BookId == bookId)
			   .OrderByDescending(r => r.CreatedAt)
			   .ToList();

			return reviews;
		}


		public void UpdateReview(Review review)
		{
			_context.Reviews.Update(review);
		}

		public double GetAverageRatingByBookId(int bookId)
		{
			var rating = _context.Reviews
				.Where(r => r.BookId == bookId)
				.Select(r => r.Rating)
				.Average();

			return rating;
		}
	}
}
