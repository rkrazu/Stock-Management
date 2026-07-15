using System;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Controls
{
    public class SupplierSelectControl : SearchableSelectControl<Supplier>
    {
        public SupplierSelectControl()
        {
            FormatItem = FormatSupplier;
        }

        public event EventHandler SelectedSupplierChanged
        {
            add => SelectedItemChanged += value;
            remove => SelectedItemChanged -= value;
        }

        public Supplier SelectedSupplier => SelectedItem;

        public void BindSearch(Func<string, System.Collections.Generic.IEnumerable<Supplier>> search, Func<Supplier, string, bool> matches = null)
        {
            Bind(search, matches);
        }

        public bool TrySelectHighlightedSupplier() => TrySelectHighlightedItem();

        private static string FormatSupplier(Supplier supplier)
        {
            if (supplier == null)
                return string.Empty;

            return string.IsNullOrWhiteSpace(supplier.Phone)
                ? supplier.Name
                : $"{supplier.Name} — {supplier.Phone}";
        }
    }
}
