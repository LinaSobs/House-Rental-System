using System.ComponentModel.DataAnnotations;

namespace HouseRentalSystem.Models.ViewModels
{
    public class PaymentViewModel
    {
        public int ApplicationId { get; set; }

        [Display(Name = "Property")]
        public string HouseTitle { get; set; } = string.Empty;

        [Display(Name = "Monthly Rent")]
        public decimal MonthlyRent { get; set; }

        [Display(Name = "Security Deposit")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Total Amount Due")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Payment Due Date")]
        public DateTime PaymentDueDate { get; set; }

        [Required]
        [Display(Name = "Cardholder Name")]
        public string CardholderName { get; set; } = string.Empty;

        // Stripe Token (will be set by Stripe Elements)
        [Required]
        public string StripeToken { get; set; } = string.Empty;

        public string StripePublishableKey { get; set; } = string.Empty;
        public string StripeClientSecret { get; set; } = string.Empty;
    }
}