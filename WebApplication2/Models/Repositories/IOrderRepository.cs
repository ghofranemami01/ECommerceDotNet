using WebApplication2.Models;

namespace WebApplication2.Models.Repositories
{
    public interface IOrderRepository
    {
        Order? GetById(int Id);
        IList<Order> GetByUserId(string userId);
        IList<Order> GetAll();
        void Add(Order o);
        Order? Update(Order order);
        void Delete(int id);
    }
}

