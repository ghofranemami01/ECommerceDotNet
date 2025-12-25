using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class ProductVariant
    {
        public int ProductVariantId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        [StringLength(50)]
        [Display(Name = "Taille")]
        public string? Size { get; set; }

        [StringLength(50)]
        [Display(Name = "Couleur")]
        public string? Color { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Stock")]
        public int Stock { get; set; }

        [Display(Name = "Prix additionnel")]
        public float? AdditionalPrice { get; set; } = 0;
    }
}



