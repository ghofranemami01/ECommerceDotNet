using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IReviewRepository
    {
        Review? GetById(int id);
        IList<Review> GetByProductId(int productId, bool onlyApproved = true);
        IList<Review> GetAllPending();
        bool HasUserReviewedProduct(int productId, string userId);
        float GetAverageRating(int productId);
        int GetReviewCount(int productId);
        void Add(Review review);
        Review? Update(Review review);
        void Delete(int id);
        void Approve(int id);
        void Reject(int id);
        IEnumerable<Review> GetAll();
      
    }
}
