using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HouseRentalSystem.Models;

namespace HouseRentalSystem.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "First Name is required")]
            [Display(Name = "First Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string FirstName { get; set; }

            [Required(ErrorMessage = "Last Name is required")]
            [Display(Name = "Last Name")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Date of Birth is required")]
            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            [MinimumAge(18, ErrorMessage = "You must be at least 18 years old to register")]
            public DateTime DateOfBirth { get; set; }

            [Required(ErrorMessage = "Phone Number is required")]
            [Display(Name = "Phone Number")]
            [Phone(ErrorMessage = "Invalid phone number")]
            [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
            public string PhoneNumber { get; set; }

            [Required(ErrorMessage = "Address is required")]
            [Display(Name = "Address")]
            [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
            public string Address { get; set; }

            [Required(ErrorMessage = "Please specify if you are a house owner")]
            [Display(Name = "I want to register as a House Owner")]
            public bool IsHouseOwner { get; set; }

            // House Owner specific fields
            [Display(Name = "Company Name")]
            [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
            public string? CompanyName { get; set; }

            [Display(Name = "Tax ID")]
            [StringLength(50, ErrorMessage = "Tax ID cannot exceed 50 characters")]
            public string? TaxId { get; set; }

            // Tenant specific fields
            [Display(Name = "Emergency Contact")]
            [StringLength(15, ErrorMessage = "Emergency contact cannot exceed 15 characters")]
            public string? EmergencyContact { get; set; }

            [Display(Name = "Occupation")]
            [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
            public string? Occupation { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Please confirm your password")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        // Custom validation attribute for minimum age
        public class MinimumAgeAttribute : ValidationAttribute
        {
            private readonly int _minimumAge;

            public MinimumAgeAttribute(int minimumAge)
            {
                _minimumAge = minimumAge;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateTime dateOfBirth)
                {
                    if (dateOfBirth.AddYears(_minimumAge) > DateTime.Today)
                    {
                        return new ValidationResult(ErrorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.DateOfBirth = Input.DateOfBirth;
                user.PhoneNumber = Input.PhoneNumber;
                user.Address = Input.Address;
                user.IsHouseOwner = Input.IsHouseOwner;

                // Set role-specific properties
                if (Input.IsHouseOwner)
                {
                    user.CompanyName = Input.CompanyName;
                    user.TaxId = Input.TaxId;
                    user.EmergencyContact = null;
                    user.Occupation = null;
                }
                else
                {
                    user.EmergencyContact = Input.EmergencyContact;
                    user.Occupation = Input.Occupation;
                    user.CompanyName = null;
                    user.TaxId = null;
                }

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}