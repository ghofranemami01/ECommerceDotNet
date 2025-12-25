using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WebApplication2.Models.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context)
        {
            this.context = context;
        }

        public IList<Product> GetAll()
        {
            return context.Products.Include(p => p.Category).ToList();
        }

        public Product GetById(int id)
        {
            return context.Products
                          .Include(p => p.Category)
                          .SingleOrDefault(p => p.ProductId == id);
        }

        public void Add(Product p)
        {
            context.Products.Add(p);
            context.SaveChanges();
        }

        public IList<Product> FindByName(string name)
        {
            return context.Products
                          .Include(p => p.Category)
                          .Where(p => p.Name.Contains(name) ||
                                      (p.Category != null && p.Category.CategoryName.Contains(name)))
                          .ToList();
        }

        public Product Update(Product p)
        {
            Product p1 = context.Products.Find(p.ProductId);
            if (p1 != null)
            {
                p1.Name = p.Name;
                p1.Price = p.Price;
                p1.QteStock = p.QteStock;
                p1.CategoryId = p.CategoryId;
                context.SaveChanges();
            }
            return p1;
        }

        public void Delete(int ProductId)
        {
            Product p1 = context.Products.Find(ProductId);
            if (p1 != null)
            {
                context.Products.Remove(p1);
                context.SaveChanges();
            }
        }

        public IList<Product> GetProductsByCategID(int? CategId)
        {
            return context.Products
                          .Include(p => p.Category)
                          .Where(p => p.CategoryId == CategId)
                          .OrderBy(p => p.ProductId)
                          .ToList();
        }

        public IQueryable<Product> GetAllProducts()
        {
            // Correction : utiliser le bon champ 'context'
            return context.Products.Include(p => p.Category);
        }
    }
}
