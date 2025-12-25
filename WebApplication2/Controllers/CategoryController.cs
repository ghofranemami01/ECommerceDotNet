using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;

namespace GestionArticles.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategorieRepository categorieRepository;

        public CategoryController(ICategorieRepository categorieRepository)
        {
            this.categorieRepository = categorieRepository;
        }

        // GET: CategoryController
        [AllowAnonymous]
        public ActionResult Index()
        {
            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(categories);
        }

        // GET: CategoryController/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var category = categorieRepository.GetById(id);
            if (category == null)
                return NotFound();

            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(category);
        }

        // GET: CategoryController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                categorieRepository.Add(category);
                return RedirectToAction(nameof(Index));
            }

            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(category);
        }

        // GET: CategoryController/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var category = categorieRepository.GetById(id);
            if (category == null)
                return NotFound();

            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(category);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Category newCategory)
        {
            if (ModelState.IsValid)
            {
                newCategory.CategoryId = id;
                categorieRepository.Update(newCategory);
                return RedirectToAction(nameof(Index));
            }

            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(newCategory);
        }

        // GET: CategoryController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            var category = categorieRepository.GetById(id);
            if (category == null)
                return NotFound();

            var categories = categorieRepository.GetAll();       // ✅ Charger les catégories
            ViewData["Categories"] = categories;

            return View(category);
        }

        // POST: CategoryController/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            categorieRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
