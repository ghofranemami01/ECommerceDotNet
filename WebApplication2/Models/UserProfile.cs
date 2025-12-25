using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public IdentityUser? User { get; set; }

        [StringLength(100)]
        [Display(Name = "Prénom")]
        public string? FirstName { get; set; }

        [StringLength(100)]
        [Display(Name = "Nom")]
        public string? LastName { get; set; }

        [StringLength(20)]
        [Display(Name = "Téléphone")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Date de naissance")]
        public DateTime? DateOfBirth { get; set; }

        public ICollection<Address> Addresses { get; set; } = new List<Address>();
    }
}



