using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models.Repositories;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(
            IReviewRepository reviewRepo,
            UserManager<IdentityUser> userManager)
        {
            _reviewRepo = reviewRepo;
            _userManager = userManager;
        }

        // ✅ PAGE DE MODÉRATION → Index.cshtml
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var reviews = _reviewRepo.GetAll().ToList();

            foreach (var review in reviews)
            {
                var user = await _userManager.FindByIdAsync(review.UserId);
                review.UserName = user?.UserName ?? review.UserId;
            }

            return View(reviews); // 👉 Views/Review/Index.cshtml
        }

        // ✅ APPROUVER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int id)
        {
            _reviewRepo.Approve(id);
            TempData["SuccessMessage"] = "Avis approuvé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // ❌ REJETER
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int id)
        {
            _reviewRepo.Reject(id);
            TempData["SuccessMessage"] = "Avis rejeté avec succès.";
            return RedirectToAction(nameof(Index));
        }
    }
}
