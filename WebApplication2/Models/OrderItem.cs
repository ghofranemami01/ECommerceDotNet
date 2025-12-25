namespace WebApplication2.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int? ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; }
        public string? VariantInfo { get; set; } // "Taille: M, Couleur: Rouge"
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}

