using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; } // "Pending", "Paid", "Failed"
        public string OrderStatus { get; set; } = "Pending"; // "Pending", "Paid", "Shipped", "Delivered", "Cancelled"
        public float TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? DeliveredDate { get; set; }
        // Lien avec l'utilisateur dans Identity
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
        // Liste des articles de la commande
        public List<OrderItem> Items { get; set; }
    }
}

