using System;
namespace ORM_Dapper.Repositories
{
	public interface IReviewRepository
	{
		public IEnumerable<Review> GetAllReviews();
		public void NewReview(int productID, string reviewer, int rating, string comment);
		public void ModifyReview(Review review, string comment);
		public void ModifyReview(Review review, int rating);
		public void DeleteReview(Review review);
	}
}

