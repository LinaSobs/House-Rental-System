using System.ComponentModel.DataAnnotations;

namespace HouseRentalSystem.Models.ViewModels
{
    public class HouseViewModel
    {
        public int HouseId { get; set; }

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
        public decimal Area { get; set; }

        [Required]
        [Range(0.01, 100000)]
        public decimal Price { get; set; }

        // Image uploads
        [Display(Name = "Main Image")]
        public IFormFile? Image1 { get; set; }

        [Display(Name = "Second Image")]
        public IFormFile? Image2 { get; set; }

        [Display(Name = "Third Image")]
        public IFormFile? Image3 { get; set; }

        // Features
        public bool HasParking { get; set; }
        public bool HasGarden { get; set; }
        public bool HasPool { get; set; }
        public bool HasFurniture { get; set; }
        public bool HasWiFi { get; set; }
        public bool PetsAllowed { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}