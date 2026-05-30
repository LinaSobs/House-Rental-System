using HouseRentalSystem.Data;
using HouseRentalSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace HouseRentalSystem.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationDbContext context,
                               UserManager<ApplicationUser> userManager,
                               IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: Payment/CreateCheckoutSession
        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession(int applicationId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                // Get the application
                var application = await _context.RentalApplications
                    .Include(ra => ra.House)
                    .Include(ra => ra.Tenant)
                    .FirstOrDefaultAsync(ra => ra.ApplicationId == applicationId && ra.TenantId == userId);

                if (application == null)
                {
                    TempData["Error"] = "Application not found.";
                    return RedirectToAction("MyApplications", "Houses");
                }

                if (application.Status != ApplicationStatus.PaymentPending)
                {
                    TempData["Error"] = "Payment is not required for this application.";
                    return RedirectToAction("MyApplications", "Houses");
                }

                if (application.PaymentCompleted)
                {
                    TempData["Info"] = "Payment has already been completed for this application.";
                    return RedirectToAction("MyApplications", "Houses");
                }

                // Get base URL
                var request = HttpContext.Request;
                var baseUrl = $"{request.Scheme}://{request.Host}";

                // Create Stripe session options
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(application.AmountDue * 100), // Convert to cents
                                Currency = "zar",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Rental Deposit - {application.House.Title}",
                                    Description = $"Security deposit and first month's rent for {application.House.Address}, {application.House.City}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    Mode = "payment",
                    SuccessUrl = $"{baseUrl}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{baseUrl}/Payment/Cancel",
                    CustomerEmail = application.Tenant.Email,
                    Metadata = new Dictionary<string, string>
                    {
                        { "applicationId", application.ApplicationId.ToString() },
                        { "houseId", application.HouseId.ToString() },
                        { "tenantId", application.TenantId }
                    }
                };

                // Set Stripe API key
                StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

                // Create the session
                var service = new SessionService();
                Session session = service.Create(options);

                // Save Stripe session ID to application
                application.StripeSessionId = session.Id;
                _context.RentalApplications.Update(application);
                await _context.SaveChangesAsync();

                return Redirect(session.Url);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "There was an error processing your payment. Please try again.";
                return RedirectToAction("MyApplications", "Houses");
            }
        }

        // GET: Payment/Success
        public async Task<IActionResult> Success(string session_id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(session_id))
            {
                try
                {
                    // Verify payment and update application status
                    StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
                    var service = new SessionService();
                    var session = service.Get(session_id);

                    if (session.PaymentStatus == "paid")
                    {
                        // Find application and update status
                        var application = await _context.RentalApplications
                            .Include(ra => ra.House)
                            .FirstOrDefaultAsync(ra => ra.StripeSessionId == session_id && ra.TenantId == userId);

                        if (application != null)
                        {
                            application.PaymentCompleted = true;
                            application.PaymentDate = DateTime.UtcNow;
                            application.Status = ApplicationStatus.Completed;

                            // Mark house as rented
                            application.House.IsAvailable = false;
                            application.House.UpdatedAt = DateTime.UtcNow;

                            _context.RentalApplications.Update(application);
                            _context.Houses.Update(application.House);
                            await _context.SaveChangesAsync();

                            TempData["Success"] = "Payment completed successfully! Your rental is now confirmed.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue to success page
                    Console.WriteLine($"Payment verification error: {ex.Message}");
                }
            }

            return View();
        }

        // GET: Payment/Cancel
        public IActionResult Cancel()
        {
            TempData["Info"] = "Payment was cancelled. You can try again later.";
            return View();
        }

        // GET: Payment/Details/5 - Show payment details before checkout
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var application = await _context.RentalApplications
                .Include(ra => ra.House)
                .FirstOrDefaultAsync(ra => ra.ApplicationId == id);

            if (application == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || application.TenantId != user.Id)
            {
                TempData["Error"] = "You can only view payment details for your own applications.";
                return RedirectToAction("MyApplications", "Houses");
            }

            if (application.Status != ApplicationStatus.PaymentPending)
            {
                TempData["Error"] = "Payment is not required for this application.";
                return RedirectToAction("MyApplications", "Houses");
            }

            return View(application);
        }
    }
}