using System.ComponentModel.DataAnnotations;

namespace HouseRentalSystem.Models.ViewModels
{
    public class RentalApplicationViewModel
    {
        public int HouseId { get; set; }

        public string HouseTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select your desired move-in date")]
        [Display(Name = "Desired Move-in Date")]
        [DataType(DataType.Date)]
        public DateTime DesiredMoveInDate { get; set; }

        [Required(ErrorMessage = "Please select lease duration")]
        [Display(Name = "Lease Duration (months)")]
        [Range(1, 24, ErrorMessage = "Lease duration must be between 1 and 24 months")]
        public int DesiredLeaseMonths { get; set; } = 12;

        [Display(Name = "Message to Property Owner")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string? MessageToOwner { get; set; }
    }
}