using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(new[] { "User", "Admin" });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password, string imie, string nazwisko, string role)
        {
            ViewBag.Roles = new SelectList(new[] { "User", "Admin" }, role);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email i hasło są wymagane.";
                return View();
            }

            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                ViewBag.Error = "Użytkownik o takim adresie email już istnieje.";
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                ViewBag.Error = "Wybrana rola nie istnieje.";
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                Imie = imie,
                Nazwisko = nazwisko,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Error = string.Join(" ", result.Errors.Select(e => e.Description));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (user.Email == "admin@biblioteka.pl")
            {
                TempData["Error"] = "Nie można usunąć głównego administratora.";
                return RedirectToAction(nameof(Index));
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}