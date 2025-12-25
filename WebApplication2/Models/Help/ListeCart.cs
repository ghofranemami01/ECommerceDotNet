using WebApplication2.Models;

namespace WebApplication2.Models.Help
{
    public class ListeCart
    {
        public List<Item> Items { get; private set; }
        public Coupon? AppliedCoupon { get; set; }
        public static readonly ListeCart Instance;
        static ListeCart()
        {
            Instance = new ListeCart();
            Instance.Items = new List<Item>();
        }
        protected ListeCart() { }
        public void AddItem(Product prod, ProductVariant? variant = null)
        {
            Boolean iswhat = false;
            // Create a new item to add to the cart
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId && 
                    ((a.Variant == null && variant == null) || 
                     (a.Variant != null && variant != null && a.Variant.ProductVariantId == variant.ProductVariantId)))
                {
                    a.quantite++;
                    iswhat = true;
                    return;
                }
            }
            if (iswhat == false)
            {
                Item newItem = new Item(prod);
                newItem.Variant = variant;
                newItem.quantite = 1;
                Items.Add(newItem);
            }
        }
        public void setToNUll()
        { }
        public void SetLessOneItem(Product prod)
        {
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    if (a.quantite <= 0)
                    {
                        RemoveItem(a.Prod);
                        return;
                    }
                    else
                    {
                        a.quantite--;
                        return;
                    }
                }
            }
        }
        public void SetItemQuantity(Product prod, int quantity)
        {
            if (quantity == 0)
            {
                RemoveItem(prod);
                return;
            }
            else
            {
                foreach (Item a in Items)
                {
                    if (a.Prod.ProductId == prod.ProductId)
                    {
                        a.quantite = quantity;
                        return;
                    }
                }
            }
        }
        public void RemoveItem(Product prod)
        {
            Item t = null;
            foreach (Item a in Items)
            {
                if (a.Prod.ProductId == prod.ProductId)
                {
                    t = a;
                }
            }
            if (t != null)
            {
                Items.Remove(t);
            }
        }
        public float GetSubTotal()
        {
            float subTotal = 0;
            foreach (Item i in Items)
                subTotal += i.TotalPrice;
            return (float)subTotal;
        }

        public float GetDiscount()
        {
            if (AppliedCoupon == null) return 0;
            float subtotal = GetSubTotal();
            if (AppliedCoupon.Type == "Percent")
            {
                return subtotal * (AppliedCoupon.Value / 100f);
            }
            else // Amount
            {
                return Math.Min(AppliedCoupon.Value, subtotal);
            }
        }

        public float GetTotal()
        {
            return GetSubTotal() - GetDiscount();
        }

        public void ClearCoupon()
        {
            AppliedCoupon = null;
        }
    }
}

