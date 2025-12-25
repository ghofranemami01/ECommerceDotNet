using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;
using WebApplication2.ViewModels;

namespace WebApplication2.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepo;
        private readonly ICategorieRepository _categRepo;
        private readonly IReviewRepository _reviewRepo;
        private readonly IProductVariantRepository _variantRepo;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public ProductController(
            IProductRepository productRepo,
            ICategorieRepository categRepo,
            IReviewRepository reviewRepo,
            IProductVariantRepository variantRepo,
            IWebHostEnvironment hostingEnvironment,
            UserManager<IdentityUser> userManager)
        {
            _productRepo = productRepo;
            _categRepo = categRepo;
            _reviewRepo = reviewRepo;
            _variantRepo = variantRepo;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // ================== INDEX ==================
        [AllowAnonymous]
        public ActionResult Index(int? categoryId, string? search = null, float? minPrice = null, float? maxPrice = null, string? sort = null, int page = 1)
        {
            int pageSize = 4;
            ViewData["Categories"] = _categRepo.GetAll();

            IQueryable<Product> productsQuery = _productRepo.GetAllProducts();

            if (categoryId.HasValue)
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim();
                productsQuery = productsQuery.Where(p =>
                    p.Name.Contains(term) ||
                    (p.Category != null && p.Category.CategoryName.Contains(term))
                );
            }

            if (minPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            if (maxPrice.HasValue) productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);

            productsQuery = sort switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name_asc" => productsQuery.OrderBy(p => p.Name),
                "name_desc" => productsQuery.OrderByDescending(p => p.Name),
                _ => productsQuery.OrderBy(p => p.ProductId)
            };

            var totalProducts = productsQuery.Count();
            var products = productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CategoryId = categoryId;
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;
            ViewBag.TotalResults = totalProducts;

            return View(products);
        }

        // ================== DETAILS ==================
        public async Task<ActionResult> Details(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            ViewData["Categories"] = _categRepo.GetAll();

            var variants = _variantRepo.GetByProductId(id);
            ViewBag.Variants = variants;

            ViewBag.Sizes = variants.Where(v => !string.IsNullOrEmpty(v.Size)).Select(v => v.Size).Distinct().ToList();
            ViewBag.Colors = variants.Where(v => !string.IsNullOrEmpty(v.Color)).Select(v => v.Color).Distinct().ToList();

            // Pour admin, afficher tous les avis (approuvés ou non)
            bool isAdmin = User.IsInRole("Admin");
            var reviews = _reviewRepo.GetByProductId(id, onlyApproved: !isAdmin).ToList();

            ViewBag.AverageRating = _reviewRepo.GetAverageRating(id);
            ViewBag.ReviewCount = _reviewRepo.GetReviewCount(id);

            var reviewerNames = new Dictionary<string, string>();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    ViewBag.HasReviewed = _reviewRepo.HasUserReviewedProduct(id, user.Id);
                    ViewBag.UserId = user.Id;

                    // Ajouter les avis non approuvés de l'utilisateur à la liste
                    var pendingReviews = _reviewRepo.GetByProductId(id, onlyApproved: false)
                        .Where(r => r.UserId == user.Id && !r.IsApproved)
                        .ToList();

                    foreach (var pr in pendingReviews)
                        if (!reviews.Any(r => r.ReviewId == pr.ReviewId)) reviews.Insert(0, pr);
                }
            }

            var userIds = reviews.Select(r => r.UserId).Where(uid => !string.IsNullOrEmpty(uid)).Distinct().ToList();
            foreach (var uid in userIds)
            {
                try
                {
                    var identityUser = await _userManager.FindByIdAsync(uid);
                    reviewerNames[uid] = identityUser?.UserName ?? uid;
                }
                catch
                {
                    reviewerNames[uid] = uid;
                }
            }

            ViewBag.ReviewerNames = reviewerNames;
            ViewBag.Reviews = reviews;

            return View(product);
        }

        // ================== ADD REVIEW ==================
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddReview(int productId, int rating, string? comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            if (_reviewRepo.HasUserReviewedProduct(productId, user.Id))
            {
                TempData["ReviewError"] = "Vous avez déjà laissé un avis pour ce produit.";
                return RedirectToAction("Details", new { id = productId });
            }

            if (rating < 1 || rating > 5)
            {
                TempData["ReviewError"] = "La note doit être entre 1 et 5.";
                return RedirectToAction("Details", new { id = productId });
            }

            _reviewRepo.Add(new Review
            {
                ProductId = productId,
                UserId = user.Id,
                Rating = rating,
                Comment = comment,
                IsApproved = false
            });

            TempData["ReviewSuccess"] = "Votre avis a été soumis et sera publié après modération.";
            return RedirectToAction("Details", new { id = productId });
        }

        // ================== CREATE PRODUCT ==================
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_categRepo.GetAll(), "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_categRepo.GetAll(), "CategoryId", "CategoryName", model.CategoryId);
                return View(model);
            }

            string? uniqueFileName = null;
            if (model.ImagePath != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using var fs = new FileStream(filePath, FileMode.Create);
                model.ImagePath.CopyTo(fs);
            }

            var newProduct = new Product
            {
                Name = model.Name,
                Price = model.Price,
                QteStock = model.QteStock,
                CategoryId = model.CategoryId,
                Image = uniqueFileName
            };

            _productRepo.Add(newProduct);
            return RedirectToAction("Details", new { id = newProduct.ProductId });
        }

        // ================== EDIT PRODUCT ==================
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            var categories = _categRepo.GetAll();
            ViewData["Categories"] = categories;
            ViewBag.CategoryId = new SelectList(categories, "CategoryId", "CategoryName", product.CategoryId);

            var model = new EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.Image
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(_categRepo.GetAll(), "CategoryId", "CategoryName", model.CategoryId);
                return View(model);
            }

            var product = _productRepo.GetById(model.ProductId);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Price = model.Price;
            product.QteStock = model.QteStock;
            product.CategoryId = model.CategoryId;

            if (model.ImagePath != null)
            {
                if (!string.IsNullOrEmpty(model.ExistingImagePath))
                {
                    string oldFile = Path.Combine(_hostingEnvironment.WebRootPath, "images", model.ExistingImagePath);
                    if (System.IO.File.Exists(oldFile)) System.IO.File.Delete(oldFile);
                }
                product.Image = ProcessUploadedFile(model);
            }

            _productRepo.Update(product);
            return RedirectToAction("Index");
        }

        [NonAction]
        private string ProcessUploadedFile(EditViewModel model)
        {
            if (model.ImagePath == null) return model.ExistingImagePath ?? string.Empty;

            string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            string uniqueFileName = Guid.NewGuid() + "_" + model.ImagePath.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var fs = new FileStream(filePath, FileMode.Create);
            model.ImagePath.CopyTo(fs);

            return uniqueFileName;
        }

        // ================== DELETE PRODUCT ==================
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var product = _productRepo.GetById(id);
            if (product == null) return NotFound();

            ViewData["Categories"] = _categRepo.GetAll();
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            _productRepo.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // ================== SEARCH ==================
        [AllowAnonymous]
        public ActionResult Search(string val)
        {
            return RedirectToAction("Index", new { search = val });
        }
    }
}
