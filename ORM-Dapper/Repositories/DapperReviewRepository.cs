using System;
using System.Data;
using System.Xml.Linq;
using Dapper;

namespace ORM_Dapper.Repositories
{
	public class DapperReviewRepository : IReviewRepository
	{
        private readonly IDbConnection _connection;

        public DapperReviewRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public IEnumerable<Review> GetAllReviews()
        {
            return _connection.Query<Review>("SELECT * FROM Reviews;");
        }

        public void ModifyReview(Review review, string comment)
        {
            _connection.Execute($"UPDATE Reviews SET Comment = @newComment WHERE ReviewID = {review.ReviewID};",
                new { newComment = comment });
        }

        public void ModifyReview(Review review, int rating)
        {
            _connection.Execute($"UPDATE Reviews SET Rating = @newRating WHERE ReviewID = {review.ReviewID};",
                new { newRating = rating });
        }

        public void NewReview(int productID, string reviewer, int rating, string comment)
        {
            _connection.Execute("INSERT INTO Reviews (ProductID, Reviewer, Rating, Comment) VALUES (@prodID, @name, @newRating, @newComment);",
                new { prodID = productID, name = reviewer, newRating = rating, newComment = comment });
        }

        public void DeleteReview(Review review)
        {
            _connection.Execute($"DELETE FROM Reviews WHERE ReviewID = {review.ReviewID};");
        }
    }
}

