using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        readonly AppDbContext context;
        public OrderRepository(AppDbContext context)
        {
            this.context = context;
        }
        public void Add(Order o)
        {
            context.Orders.Add(o);
            context.SaveChanges();
        }
        public Order? GetById(int id)
        {
            return context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);
        }

        public IList<Order> GetByUserId(string userId)
        {
            return context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public IList<Order> GetAll()
        {
            return context.Orders
                .Include(o => o.Items)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order? Update(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));
            var existing = context.Orders.Find(order.Id);
            if (existing != null)
            {
                existing.OrderStatus = order.OrderStatus;
                existing.PaymentStatus = order.PaymentStatus;
                existing.ShippedDate = order.ShippedDate;
                existing.DeliveredDate = order.DeliveredDate;
                context.SaveChanges();
            }
            return existing;
        }

        public void Delete(int id)
        {
            var order = context.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == id);
            if (order != null)
            {
                // Optionally restore stock for items
                context.OrderItems.RemoveRange(order.Items);
                context.Orders.Remove(order);
                context.SaveChanges();
            }
        }
    }
}



