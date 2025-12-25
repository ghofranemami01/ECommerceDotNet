using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public Review? GetById(int id) =>
            _context.Reviews.Include(r => r.Product).FirstOrDefault(r => r.ReviewId == id);

        public IList<Review> GetByProductId(int productId, bool onlyApproved = true)
        {
            var query = _context.Reviews.Include(r => r.Product).Where(r => r.ProductId == productId);
            if (onlyApproved) query = query.Where(r => r.IsApproved);
            return query.OrderByDescending(r => r.CreatedAt).ToList();
        }

        public IList<Review> GetAllPending() =>
            _context.Reviews.Include(r => r.Product).Where(r => !r.IsApproved).OrderByDescending(r => r.CreatedAt).ToList();

        public IEnumerable<Review> GetAll() =>
            _context.Reviews.Include(r => r.Product).OrderByDescending(r => r.CreatedAt).ToList();

        public bool HasUserReviewedProduct(int productId, string userId) =>
            _context.Reviews.Any(r => r.ProductId == productId && r.UserId == userId);

        public float GetAverageRating(int productId)
        {
            var reviews = _context.Reviews.Where(r => r.ProductId == productId && r.IsApproved);
            return reviews.Any() ? (float)reviews.Average(r => r.Rating) : 0;
        }

        public int GetReviewCount(int productId) =>
            _context.Reviews.Count(r => r.ProductId == productId && r.IsApproved);

        public void Add(Review review)
        {
            review.CreatedAt = DateTime.Now;
            _context.Reviews.Add(review);
            _context.SaveChanges();
        }

        public Review? Update(Review review)
        {
            var existing = _context.Reviews.Find(review.ReviewId);
            if (existing != null)
            {
                existing.Rating = review.Rating;
                existing.Comment = review.Comment;
                _context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                _context.SaveChanges();
            }
        }

        public void Approve(int id)
        {
            var review = _context.Reviews.Find(id);
            if (review != null)
            {
                review.IsApproved = true;
                _context.SaveChanges();
            }
        }

        public void Reject(int id) => Delete(id);
    }
}
