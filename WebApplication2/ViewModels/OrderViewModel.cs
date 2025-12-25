using System.ComponentModel.DataAnnotations;

namespace WebApplication2.ViewModels
{
    public class OrderViewModel
    {
        [Required]
        [Display(Name = "Adresse")]
        public string Address { get; set; } = string.Empty;

        [Display(Name = "Ville")]
        public string? City { get; set; }

        [Display(Name = "Code postal")]
        public string? PostalCode { get; set; }

        [Display(Name = "Pays")]
        public string? Country { get; set; } = "Tunisie";

        [Display(Name = "Téléphone")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Méthode de paiement")]
        public string PaymentMethod { get; set; } = "CashOnDelivery"; // "CashOnDelivery", "Stripe", "PayPal"

        public float TotalAmount { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public int? SelectedAddressId { get; set; }
        public string? Email { get; set; }
    }

    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantInfo { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}

