using System;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Controls
{
    public class ProductSelectControl : SearchableSelectControl<Product>
    {
        public ProductSelectControl()
        {
            FormatItem = FormatProduct;
        }

        public event EventHandler SelectedProductChanged
        {
            add => SelectedItemChanged += value;
            remove => SelectedItemChanged -= value;
        }

        public Product SelectedProduct => SelectedItem;

        public void BindSearch(Func<string, System.Collections.Generic.IEnumerable<Product>> search, Func<Product, string, bool> matches = null)
        {
            Bind(search, matches);
        }

        public bool TrySelectHighlightedProduct() => TrySelectHighlightedItem();

        public void SelectProduct(Product product) => SetSelectedItem(product);

        private static string FormatProduct(Product product)
        {
            if (product == null)
                return string.Empty;

            return string.IsNullOrWhiteSpace(product.Sku)
                ? product.Name
                : $"{product.Name} — {product.Sku}";
        }
    }
}
