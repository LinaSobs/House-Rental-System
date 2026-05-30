using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HouseRentalSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        // Use 'new' keyword to hide the base property
        [Required]
        [StringLength(15)]
        public new string? PhoneNumber { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Role-specific properties
        public bool IsHouseOwner { get; set; } = false;

        // Additional properties for house owners
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }

        // Additional properties for tenants
        public string? EmergencyContact { get; set; }
        public string? Occupation { get; set; }
    }
}