using WebApplication2.Models;

namespace WebApplication2.Models.Help
{
    public class Item
    {
        public int quantite { get; set; }
        private int _ProduitId;
        public Product _product = null;
        public ProductVariant? _variant = null;
        public Product Prod
        {
            get { return _product; }
            set { _product = value; }
        }
        public ProductVariant? Variant
        {
            get { return _variant; }
            set { _variant = value; }
        }
        public string Description
        {
            get 
            { 
                string desc = _product.Name;
                if (_variant != null)
                {
                    if (!string.IsNullOrEmpty(_variant.Size))
                        desc += $" - Taille: {_variant.Size}";
                    if (!string.IsNullOrEmpty(_variant.Color))
                        desc += $" - Couleur: {_variant.Color}";
                }
                return desc;
            }
        }
        public float UnitPrice
        {
            get 
            { 
                float price = _product.Price;
                if (_variant != null && _variant.AdditionalPrice.HasValue)
                {
                    price += _variant.AdditionalPrice.Value;
                }
                return price;
            }
        }
        public int categoryId
        {
            get { return _product.CategoryId; }
        }
        public Category category
        {
            get { return _product.Category; }
        }
        public float TotalPrice
        {
            get { return _product.Price * quantite; }
        }
        public Item(Product p)
        {
            this.Prod = p;
        }
        public bool Equals(Item item)
        {
            if (item.Prod.ProductId != this.Prod.ProductId)
                return false;
            
            // Comparer les variantes
            if (this.Variant == null && item.Variant == null)
                return true;
            
            if (this.Variant != null && item.Variant != null)
                return this.Variant.ProductVariantId == item.Variant.ProductVariantId;
            
            return false;
        }
    }
}

