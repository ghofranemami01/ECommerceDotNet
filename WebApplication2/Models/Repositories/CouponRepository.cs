using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        readonly AppDbContext context;

        public CouponRepository(AppDbContext context)
        {
            this.context = context;
        }

        public Coupon? GetByCode(string code)
        {
            return context.Coupons
                .FirstOrDefault(c => c.Code.ToUpper() == code.ToUpper());
        }

        public Coupon? GetById(int id)
        {
            return context.Coupons.Find(id);
        }

        public IList<Coupon> GetAll()
        {
            return context.Coupons.OrderBy(c => c.Code).ToList();
        }

        public void Add(Coupon coupon)
        {
            if (coupon == null) throw new ArgumentNullException(nameof(coupon));
            context.Coupons.Add(coupon);
            context.SaveChanges();
        }

        public Coupon? Update(Coupon coupon)
        {
            if (coupon == null) throw new ArgumentNullException(nameof(coupon));
            var existing = context.Coupons.Find(coupon.CouponId);
            if (existing != null)
            {
                existing.Code = coupon.Code;
                existing.Type = coupon.Type;
                existing.Value = coupon.Value;
                existing.MinOrder = coupon.MinOrder;
                existing.StartDate = coupon.StartDate;
                existing.EndDate = coupon.EndDate;
                existing.MaxUsePerUser = coupon.MaxUsePerUser;
                existing.MaxTotalUse = coupon.MaxTotalUse;
                existing.IsActive = coupon.IsActive;
                context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var coupon = context.Coupons.Find(id);
            if (coupon != null)
            {
                context.Coupons.Remove(coupon);
                context.SaveChanges();
            }
        }

        public bool ValidateCoupon(Coupon coupon, float orderTotal, string? userId = null)
        {
            if (coupon == null) return false;
            if (!coupon.IsActive) return false;

            // Vérifier les dates
            if (coupon.StartDate.HasValue && DateTime.Now < coupon.StartDate.Value)
                return false;
            if (coupon.EndDate.HasValue && DateTime.Now > coupon.EndDate.Value)
                return false;

            // Vérifier le montant minimum
            if (coupon.MinOrder.HasValue && orderTotal < coupon.MinOrder.Value)
                return false;

            // Vérifier les utilisations totales
            if (coupon.MaxTotalUse.HasValue && coupon.CurrentUse >= coupon.MaxTotalUse.Value)
                return false;

            return true;
        }

        public void IncrementUse(int couponId)
        {
            var coupon = context.Coupons.Find(couponId);
            if (coupon != null)
            {
                coupon.CurrentUse++;
                context.SaveChanges();
            }
        }
    }
}

