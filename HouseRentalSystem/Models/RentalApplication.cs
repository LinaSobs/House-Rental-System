using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseRentalSystem.Models
{
    public class RentalApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public int HouseId { get; set; }

        [ForeignKey("HouseId")]
        public virtual House House { get; set; } = null!;

        [Required]
        public string TenantId { get; set; } = string.Empty;

        [ForeignKey("TenantId")]
        public virtual ApplicationUser Tenant { get; set; } = null!;

        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DesiredMoveInDate { get; set; }

        [Range(1, 24)]
        public int DesiredLeaseMonths { get; set; } = 12;

        [StringLength(1000)]
        public string? MessageToOwner { get; set; }

        // Application status
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime? StatusUpdatedAt { get; set; }

        [StringLength(500)]
        public string? OwnerNotes { get; set; }

        // Payment properties for Stripe Checkout
        public bool PaymentRequired { get; set; } = false;
        public decimal AmountDue { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public bool PaymentCompleted { get; set; } = false;
        public string? StripeSessionId { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected,
        Withdrawn,
        PaymentPending,
        Completed
    }
}