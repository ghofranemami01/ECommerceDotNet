using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface ICouponRepository
    {
        Coupon? GetByCode(string code);
        Coupon? GetById(int id);
        IList<Coupon> GetAll();
        void Add(Coupon coupon);
        Coupon? Update(Coupon coupon);
        void Delete(int id);
        bool ValidateCoupon(Coupon coupon, float orderTotal, string? userId = null);
        void IncrementUse(int couponId);
    }
}

