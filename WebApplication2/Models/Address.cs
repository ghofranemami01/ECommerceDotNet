using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Address
    {
        public int AddressId { get; set; }

        [Required]
        public int UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Adresse")]
        public string StreetAddress { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Ville")]
        public string? City { get; set; }

        [StringLength(20)]
        [Display(Name = "Code postal")]
        public string? PostalCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Pays")]
        public string? Country { get; set; } = "Tunisie";

        [Display(Name = "Adresse par défaut")]
        public bool IsDefault { get; set; } = false;

        [StringLength(100)]
        [Display(Name = "Type d'adresse")]
        public string? AddressType { get; set; } // "Home", "Work", "Other"
    }
}



