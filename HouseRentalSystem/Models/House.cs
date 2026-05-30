using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentalSystem.Models
{
    public class House
    {
        [Key]
        public int HouseId { get; set; }

        [Required]
        public string OwnerId { get; set; } = string.Empty;

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string ZipCode { get; set; } = string.Empty;

        [Required]
        [Range(1, 10)]
        public int Bedrooms { get; set; }

        [Required]
        [Range(1, 5)]
        public int Bathrooms { get; set; }

        [Required]
        [Range(1, 1000)]
        public decimal Area { get; set; } // in square meters

        [Required]
        [Range(0.01, 100000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; } // per month

        // Image paths - we'll store up to 3 images
        [StringLength(500)]
        public string? ImagePath1 { get; set; }

        [StringLength(500)]
        public string? ImagePath2 { get; set; }

        [StringLength(500)]
        public string? ImagePath3 { get; set; }

        // Additional features
        public bool HasParking { get; set; }
        public bool HasGarden { get; set; }
        public bool HasPool { get; set; }
        public bool HasFurniture { get; set; }
        public bool HasWiFi { get; set; }
        public bool PetsAllowed { get; set; }

        // House status
        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property for rental requests
        public virtual ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
    }
}