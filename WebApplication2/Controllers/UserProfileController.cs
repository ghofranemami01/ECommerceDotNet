using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public UserProfileController(
            IUserProfileRepository userProfileRepository,
            IAddressRepository addressRepository,
            IOrderRepository orderRepository,
            UserManager<IdentityUser> userManager)
        {
            _userProfileRepository = userProfileRepository;
            _addressRepository = addressRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
        }

        // GET: UserProfile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var profile = _userProfileRepository.GetByUserId(user.Id);
            if (profile == null)
            {
                // Créer un profil si inexistant
                profile = new UserProfile
                {
                    UserId = user.Id,
                    FirstName = user.UserName
                };
                _userProfileRepository.Add(profile);
            }

            var addresses = _addressRepository.GetByUserProfileId(profile.UserProfileId);
            var orders = _orderRepository.GetByUserId(user.Id);

            ViewBag.Addresses = addresses;
            ViewBag.Orders = orders;
            ViewBag.User = user;

            return View(profile);
        }

        // POST: UserProfile/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UserProfile profile)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var existingProfile = _userProfileRepository.GetByUserId(user.Id);
            if (existingProfile == null)
            {
                profile.UserId = user.Id;
                _userProfileRepository.Add(profile);
            }
            else
            {
                existingProfile.FirstName = profile.FirstName;
                existingProfile.LastName = profile.LastName;
                existingProfile.PhoneNumber = profile.PhoneNumber;
                existingProfile.DateOfBirth = profile.DateOfBirth;
                _userProfileRepository.Update(existingProfile);
            }

            TempData["SuccessMessage"] = "Profil mis à jour avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserProfile/AddAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(Address address)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var profile = _userProfileRepository.GetByUserId(user.Id);
            if (profile == null)
            {
                profile = new UserProfile { UserId = user.Id };
                _userProfileRepository.Add(profile);
            }

            address.UserProfileId = profile.UserProfileId;
            _addressRepository.Add(address);

            TempData["SuccessMessage"] = "Adresse ajoutée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserProfile/UpdateAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAddress(Address address)
        {
            _addressRepository.Update(address);
            TempData["SuccessMessage"] = "Adresse mise à jour avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // POST: UserProfile/DeleteAddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int id)
        {
            _addressRepository.Delete(id);
            TempData["SuccessMessage"] = "Adresse supprimée avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: UserProfile/Orders
        public async Task<IActionResult> Orders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var orders = _orderRepository.GetByUserId(user.Id);
            return View(orders);
        }

        // GET: UserProfile/OrderDetails/{id}
        public IActionResult OrderDetails(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null) return NotFound();

            // Vérifier que la commande appartient à l'utilisateur
            if (order.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            return View(order);
        }
    }
}



