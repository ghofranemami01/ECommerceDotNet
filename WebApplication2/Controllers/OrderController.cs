using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models.Repositories;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductVariantRepository _variantRepository;
        private readonly IProductRepository _productRepository;

        public OrderController(
            IOrderRepository orderRepository,
            IProductVariantRepository variantRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _variantRepository = variantRepository;
            _productRepository = productRepository;
        }


        public IActionResult Index()
        {
            return RedirectToAction(nameof(AdminOrders));
        }


        // GET: Order/AdminOrders => Liste des commandes côté admin
        public IActionResult AdminOrders()
        {
            var orders = _orderRepository.GetAll();
            return View(orders);
        }

        // GET: Order/Details/5
        public IActionResult Details(int id)
        {
            var order = _orderRepository.GetById(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string orderStatus)
        {
            var order = _orderRepository.GetById(id);
            if (order == null) return NotFound();

            // Mettre à jour le statut
            order.OrderStatus = orderStatus;

            // Dates automatiques selon le statut
            switch (orderStatus)
            {
                case "Shipped":
                    if (!order.ShippedDate.HasValue)
                        order.ShippedDate = DateTime.Now;
                    break;
                case "Delivered":
                    if (!order.DeliveredDate.HasValue)
                        order.DeliveredDate = DateTime.Now;
                    order.PaymentStatus = "Paid";
                    break;
            }

            _orderRepository.Update(order);
            TempData["SuccessMessage"] = "Statut de la commande mis à jour avec succès.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Order/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, bool restoreStock = true)
        {
            var order = _orderRepository.GetById(id);
            if (order == null) return NotFound();

            // Vérifier que seule une commande annulée ou en attente peut être supprimée
            if (order.OrderStatus != "Cancelled" && order.OrderStatus != "Pending")
            {
                TempData["ErrorMessage"] = "Seules les commandes annulées ou en attente peuvent être supprimées.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Restaurer le stock si nécessaire
            if (restoreStock)
            {
                foreach (var item in order.Items ?? Enumerable.Empty<WebApplication2.Models.OrderItem>())
                {
                    if (item.ProductVariantId.HasValue)
                    {
                        var variant = _variantRepository.GetById(item.ProductVariantId.Value);
                        if (variant != null)
                        {
                            variant.Stock += item.Quantity;
                            _variantRepository.Update(variant);
                        }
                    }
                    else if (item.ProductId.HasValue)
                    {
                        var product = _productRepository.GetById(item.ProductId.Value);
                        if (product != null)
                        {
                            product.QteStock += item.Quantity;
                            _productRepository.Update(product);
                        }
                    }
                }
            }

            // Supprimer la commande
            _orderRepository.Delete(id);
            TempData["SuccessMessage"] = "Commande supprimée avec succès.";

            return RedirectToAction(nameof(AdminOrders));
        }
    }
}
