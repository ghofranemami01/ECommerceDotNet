using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class ProductVariantRepository : IProductVariantRepository
    {
        readonly AppDbContext context;

        public ProductVariantRepository(AppDbContext context)
        {
            this.context = context;
        }

        public ProductVariant? GetById(int id)
        {
            return context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefault(v => v.ProductVariantId == id);
        }

        public IList<ProductVariant> GetByProductId(int productId)
        {
            return context.ProductVariants
                .Where(v => v.ProductId == productId)
                .OrderBy(v => v.Size)
                .ThenBy(v => v.Color)
                .ToList();
        }

        public ProductVariant? GetByProductAndAttributes(int productId, string? size, string? color)
        {
            var query = context.ProductVariants
                .Where(v => v.ProductId == productId);

            if (!string.IsNullOrWhiteSpace(size))
            {
                query = query.Where(v => v.Size == size);
            }
            else
            {
                query = query.Where(v => v.Size == null || v.Size == "");
            }

            if (!string.IsNullOrWhiteSpace(color))
            {
                query = query.Where(v => v.Color == color);
            }
            else
            {
                query = query.Where(v => v.Color == null || v.Color == "");
            }

            return query.FirstOrDefault();
        }

        public void Add(ProductVariant variant)
        {
            if (variant == null) throw new ArgumentNullException(nameof(variant));
            context.ProductVariants.Add(variant);
            context.SaveChanges();
        }

        public ProductVariant? Update(ProductVariant variant)
        {
            if (variant == null) throw new ArgumentNullException(nameof(variant));
            var existing = context.ProductVariants.Find(variant.ProductVariantId);
            if (existing != null)
            {
                existing.Size = variant.Size;
                existing.Color = variant.Color;
                existing.SKU = variant.SKU;
                existing.Stock = variant.Stock;
                existing.AdditionalPrice = variant.AdditionalPrice;
                context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var variant = context.ProductVariants.Find(id);
            if (variant != null)
            {
                context.ProductVariants.Remove(variant);
                context.SaveChanges();
            }
        }

        public void DecrementStock(int variantId, int quantity)
        {
            var variant = context.ProductVariants.Find(variantId);
            if (variant != null && variant.Stock >= quantity)
            {
                variant.Stock -= quantity;
                context.SaveChanges();
            }
        }
    }
}



