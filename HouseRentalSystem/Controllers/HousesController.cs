using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HouseRentalSystem.Models;
using HouseRentalSystem.Models.ViewModels;
using HouseRentalSystem.Data;
using Stripe;

namespace HouseRentalSystem.Controllers
{
    [Authorize]
    public class HousesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public HousesController(ApplicationDbContext context,
                              UserManager<ApplicationUser> userManager,
                              IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // ===== HOUSE OWNER METHODS =====

        // GET: Houses/MyHouses - Show houses for the current owner
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!user.IsHouseOwner)
            {
                TempData["Error"] = "You must be a house owner to access this page.";
                return RedirectToAction("Index", "Home");
            }

            var houses = await _context.Houses
                .Where(h => h.OwnerId == user.Id)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return View(houses);
        }

        // GET: Houses/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsHouseOwner)
            {
                TempData["Error"] = "You must be a house owner to list properties.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Houses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HouseViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsHouseOwner)
            {
                TempData["Error"] = "You must be a house owner to list properties.";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                var house = new House
                {
                    OwnerId = user.Id,
                    Title = model.Title!,
                    Description = model.Description!,
                    Address = model.Address!,
                    City = model.City!,
                    ZipCode = model.ZipCode!,
                    Bedrooms = model.Bedrooms,
                    Bathrooms = model.Bathrooms,
                    Area = model.Area,
                    Price = model.Price,
                    HasParking = model.HasParking,
                    HasGarden = model.HasGarden,
                    HasPool = model.HasPool,
                    HasFurniture = model.HasFurniture,
                    HasWiFi = model.HasWiFi,
                    PetsAllowed = model.PetsAllowed,
                    IsAvailable = model.IsAvailable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Handle image uploads
                house.ImagePath1 = await SaveImage(model.Image1);
                house.ImagePath2 = await SaveImage(model.Image2);
                house.ImagePath3 = await SaveImage(model.Image3);

                _context.Houses.Add(house);
                await _context.SaveChangesAsync();

                TempData["Success"] = "House listed successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Houses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses.FindAsync(id);
            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || house.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only edit your own properties.";
                return RedirectToAction(nameof(Index));
            }

            var model = new HouseViewModel
            {
                HouseId = house.HouseId,
                Title = house.Title,
                Description = house.Description,
                Address = house.Address,
                City = house.City,
                ZipCode = house.ZipCode,
                Bedrooms = house.Bedrooms,
                Bathrooms = house.Bathrooms,
                Area = house.Area,
                Price = house.Price,
                HasParking = house.HasParking,
                HasGarden = house.HasGarden,
                HasPool = house.HasPool,
                HasFurniture = house.HasFurniture,
                HasWiFi = house.HasWiFi,
                PetsAllowed = house.PetsAllowed,
                IsAvailable = house.IsAvailable
            };

            return View(model);
        }

        // POST: Houses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HouseViewModel model)
        {
            if (id != model.HouseId) return NotFound();

            var house = await _context.Houses.FindAsync(id);
            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || house.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only edit your own properties.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    house.Title = model.Title!;
                    house.Description = model.Description!;
                    house.Address = model.Address!;
                    house.City = model.City!;
                    house.ZipCode = model.ZipCode!;
                    house.Bedrooms = model.Bedrooms;
                    house.Bathrooms = model.Bathrooms;
                    house.Area = model.Area;
                    house.Price = model.Price;
                    house.HasParking = model.HasParking;
                    house.HasGarden = model.HasGarden;
                    house.HasPool = model.HasPool;
                    house.HasFurniture = model.HasFurniture;
                    house.HasWiFi = model.HasWiFi;
                    house.PetsAllowed = model.PetsAllowed;
                    house.IsAvailable = model.IsAvailable;
                    house.UpdatedAt = DateTime.UtcNow;

                    // Update images if new ones are provided
                    if (model.Image1 != null)
                        house.ImagePath1 = await SaveImage(model.Image1);
                    if (model.Image2 != null)
                        house.ImagePath2 = await SaveImage(model.Image2);
                    if (model.Image3 != null)
                        house.ImagePath3 = await SaveImage(model.Image3);

                    _context.Houses.Update(house);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "House updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HouseExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }

        // GET: Houses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses
                .Include(h => h.RentalApplications)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || house.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only delete your own properties.";
                return RedirectToAction(nameof(Index));
            }

            return View(house);
        }

        // POST: Houses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var house = await _context.Houses
                .Include(h => h.RentalApplications)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || house.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only delete your own properties.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Delete associated images from the file system
                await DeleteImageFiles(house);

                // Remove the house (this will cascade delete rental applications due to EF Core configuration)
                _context.Houses.Remove(house);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Property deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while deleting the property: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Houses/Applications/5 - View rental applications for a specific house
        public async Task<IActionResult> Applications(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses
                .Include(h => h.RentalApplications)
                    .ThenInclude(ra => ra.Tenant)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || house.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only view applications for your own properties.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.HouseTitle = house.Title;
            ViewBag.HouseId = house.HouseId;

            return View(house.RentalApplications.ToList());
        }

        // POST: Houses/UpdateApplicationStatus - Updated for Stripe Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, ApplicationStatus status, string? ownerNotes)
        {
            var application = await _context.RentalApplications
                .Include(ra => ra.House)
                .FirstOrDefaultAsync(ra => ra.ApplicationId == applicationId);

            if (application == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null || application.House.OwnerId != user.Id)
            {
                TempData["Error"] = "You can only update applications for your own properties.";
                return RedirectToAction(nameof(Index));
            }

            application.Status = status;
            application.StatusUpdatedAt = DateTime.UtcNow;
            application.OwnerNotes = ownerNotes;

            // If application is approved, set up payment requirements
            if (status == ApplicationStatus.Approved)
            {
                application.PaymentRequired = true;
                application.AmountDue = application.House.Price * 2; // First month + security deposit
                application.PaymentDueDate = DateTime.UtcNow.AddDays(7); // 7 days to pay
                application.Status = ApplicationStatus.PaymentPending;
            }

            _context.RentalApplications.Update(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Application {status.ToString().ToLower()} successfully!";
            return RedirectToAction(nameof(Applications), new { id = application.HouseId });
        }
        // ===== TENANT/PUBLIC METHODS =====

        // ===== SEARCH AND BROWSE METHODS =====

        [AllowAnonymous]
        public async Task<IActionResult> Browse(string searchString, string city, int? minBedrooms, int? maxPrice, string sortBy = "newest")
        {
            var housesQuery = _context.Houses
                .Where(h => h.IsAvailable)
                .Include(h => h.Owner)
                .AsQueryable();

            // Apply search filters
            if (!string.IsNullOrEmpty(searchString))
            {
                housesQuery = housesQuery.Where(h =>
                    h.Title.Contains(searchString) ||
                    h.Description.Contains(searchString) ||
                    h.Address.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(city))
            {
                housesQuery = housesQuery.Where(h => h.City.Contains(city));
            }

            if (minBedrooms.HasValue)
            {
                housesQuery = housesQuery.Where(h => h.Bedrooms >= minBedrooms.Value);
            }

            if (maxPrice.HasValue)
            {
                housesQuery = housesQuery.Where(h => h.Price <= maxPrice.Value);
            }

            // Apply sorting
            housesQuery = sortBy.ToLower() switch
            {
                "price_low" => housesQuery.OrderBy(h => h.Price),
                "price_high" => housesQuery.OrderByDescending(h => h.Price),
                "bedrooms" => housesQuery.OrderByDescending(h => h.Bedrooms),
                "size" => housesQuery.OrderByDescending(h => h.Area),
                _ => housesQuery.OrderByDescending(h => h.CreatedAt) // newest first
            };

            var houses = await housesQuery.ToListAsync();

            // Pass search parameters to view to maintain form state
            ViewBag.SearchString = searchString;
            ViewBag.City = city;
            ViewBag.MinBedrooms = minBedrooms;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;

            return View(houses);
        }

        // GET: Houses/Details/5 - Public details view for tenants (REMOVED DUPLICATE)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses
                .Include(h => h.Owner)
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null) return NotFound();

            // Return different views based on user role
            var user = await _userManager.GetUserAsync(User);
            if (user != null && user.IsHouseOwner && house.OwnerId == user.Id)
            {
                // Owner viewing their own property - use owner details view
                return View("Details", house);
            }
            else
            {
                // Tenant or public user - use public details view
                return View("HouseDetails", house);
            }
        }

        // GET: Houses/Apply/5 - Show application form
        [Authorize]
        public async Task<IActionResult> Apply(int? id)
        {
            if (id == null) return NotFound();

            var house = await _context.Houses
                .FirstOrDefaultAsync(h => h.HouseId == id);

            if (house == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Check if user already applied for this house
            var existingApplication = await _context.RentalApplications
                .FirstOrDefaultAsync(ra => ra.HouseId == id && ra.TenantId == user.Id);

            if (existingApplication != null)
            {
                TempData["Info"] = "You have already applied for this property.";
                return RedirectToAction(nameof(MyApplications));
            }

            var model = new RentalApplicationViewModel
            {
                HouseId = house.HouseId,
                HouseTitle = house.Title,
                DesiredMoveInDate = DateTime.Today.AddDays(30), // Default to 30 days from now
                DesiredLeaseMonths = 12
            };

            return View(model);
        }

        // POST: Houses/Apply - Submit rental application
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Apply(RentalApplicationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var house = await _context.Houses.FindAsync(model.HouseId);
            if (house == null)
            {
                ModelState.AddModelError("", "Property not found.");
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Check if user already applied for this house
            var existingApplication = await _context.RentalApplications
                .FirstOrDefaultAsync(ra => ra.HouseId == model.HouseId && ra.TenantId == user.Id);

            if (existingApplication != null)
            {
                TempData["Info"] = "You have already applied for this property.";
                return RedirectToAction(nameof(MyApplications));
            }

            var application = new RentalApplication
            {
                HouseId = model.HouseId,
                TenantId = user.Id,
                DesiredMoveInDate = model.DesiredMoveInDate,
                DesiredLeaseMonths = model.DesiredLeaseMonths,
                MessageToOwner = model.MessageToOwner,
                ApplicationDate = DateTime.UtcNow,
                Status = ApplicationStatus.Pending
            };

            _context.RentalApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Your application has been submitted successfully!";
            return RedirectToAction(nameof(MyApplications));
        }

        // GET: Houses/MyApplications - Show user's rental applications
        [Authorize]
        public async Task<IActionResult> MyApplications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var applications = await _context.RentalApplications
                .Include(ra => ra.House)
                .ThenInclude(h => h.Owner)
                .Where(ra => ra.TenantId == user.Id)
                .OrderByDescending(ra => ra.ApplicationDate)
                .ToListAsync();

            return View(applications);
        }

        // ===== HELPER METHODS =====

        // Helper method to save images
        private async Task<string?> SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "houses");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return $"/images/houses/{uniqueFileName}";
        }

        // Helper method to delete image files
        private async Task DeleteImageFiles(House house)
        {
            var tasks = new List<Task>();

            if (!string.IsNullOrEmpty(house.ImagePath1))
            {
                tasks.Add(Task.Run(() =>
                {
                    var filePath = Path.Combine(_environment.WebRootPath, house.ImagePath1.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }));
            }

            if (!string.IsNullOrEmpty(house.ImagePath2))
            {
                tasks.Add(Task.Run(() =>
                {
                    var filePath = Path.Combine(_environment.WebRootPath, house.ImagePath2.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }));
            }

            if (!string.IsNullOrEmpty(house.ImagePath3))
            {
                tasks.Add(Task.Run(() =>
                {
                    var filePath = Path.Combine(_environment.WebRootPath, house.ImagePath3.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }));
            }

            await Task.WhenAll(tasks);
        }

        // Helper method to check if house exists
        private bool HouseExists(int id)
        {
            return _context.Houses.Any(e => e.HouseId == id);
        }
    }
}