using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Coupon
    {
        public int CouponId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Code")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Type")]
        public string Type { get; set; } = "Percent"; // "Percent" ou "Amount"

        [Required]
        [Display(Name = "Valeur")]
        public float Value { get; set; }

        [Display(Name = "Commande minimum")]
        public float? MinOrder { get; set; }

        [Display(Name = "Date de début")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Date de fin")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Utilisations max par utilisateur")]
        public int? MaxUsePerUser { get; set; }

        [Display(Name = "Utilisations totales max")]
        public int? MaxTotalUse { get; set; }

        [Display(Name = "Utilisations actuelles")]
        public int CurrentUse { get; set; } = 0;

        [Display(Name = "Actif")]
        public bool IsActive { get; set; } = true;
    }
}

