using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication2.Models;
using WebApplication2.Models.Repositories;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CouponController : Controller
    {
        private readonly ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        // GET: CouponController
        public ActionResult Index()
        {
            var coupons = _couponRepository.GetAll();
            return View(coupons);
        }

        // GET: CouponController/Details/5
        public ActionResult Details(int id)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                return NotFound();
            return View(coupon);
        }

        // GET: CouponController/Create
        public ActionResult Create()
        {
            ViewBag.TypeList = new SelectList(new[]
            {
                new { Value = "Percent", Text = "Pourcentage (%)" },
                new { Value = "Amount", Text = "Montant fixe" }
            }, "Value", "Text");
            return View();
        }

        // POST: CouponController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                _couponRepository.Add(coupon);
                TempData["SuccessMessage"] = "Coupon créé avec succès.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.TypeList = new SelectList(new[]
            {
                new { Value = "Percent", Text = "Pourcentage (%)" },
                new { Value = "Amount", Text = "Montant fixe" }
            }, "Value", "Text", coupon.Type);
            return View(coupon);
        }

        // GET: CouponController/Edit/5
        public ActionResult Edit(int id)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                return NotFound();
            ViewBag.TypeList = new SelectList(new[]
            {
                new { Value = "Percent", Text = "Pourcentage (%)" },
                new { Value = "Amount", Text = "Montant fixe" }
            }, "Value", "Text", coupon.Type);
            return View(coupon);
        }

        // POST: CouponController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Coupon coupon)
        {
            if (id != coupon.CouponId)
                return NotFound();

            if (ModelState.IsValid)
            {
                var updated = _couponRepository.Update(coupon);
                if (updated != null)
                {
                    TempData["SuccessMessage"] = "Coupon modifié avec succès.";
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }
            ViewBag.TypeList = new SelectList(new[]
            {
                new { Value = "Percent", Text = "Pourcentage (%)" },
                new { Value = "Amount", Text = "Montant fixe" }
            }, "Value", "Text", coupon.Type);
            return View(coupon);
        }

        // GET: CouponController/Delete/5
        public ActionResult Delete(int id)
        {
            var coupon = _couponRepository.GetById(id);
            if (coupon == null)
                return NotFound();
            return View(coupon);
        }

        // POST: CouponController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _couponRepository.Delete(id);
            TempData["SuccessMessage"] = "Coupon supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }
    }
}

