using Bibliotheca.Models;

namespace Bibliotheca.Interfaces
{
	public interface IReviewService
	{
		IEnumerable<Review> GetAllReviews();
		Review GetReviewById(int id);
		void AddReview(Review review);
		void UpdateReview(Review review);
		void DeleteReview(int id);
		IEnumerable<Review> GetReviewsByBookId(int bookId);
		double GetAverageRatingByBookId(int bookId);
	}
}
