using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IProductVariantRepository
    {
        ProductVariant? GetById(int id);
        IList<ProductVariant> GetByProductId(int productId);
        ProductVariant? GetByProductAndAttributes(int productId, string? size, string? color);
        void Add(ProductVariant variant);
        ProductVariant? Update(ProductVariant variant);
        void Delete(int id);
        void DecrementStock(int variantId, int quantity);
    }
}



