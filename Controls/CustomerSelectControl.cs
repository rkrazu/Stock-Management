using System;
using Stock_Managemnet.Models;

namespace Stock_Managemnet.Controls
{
    public class CustomerSelectControl : SearchableSelectControl<Customer>
    {
        public CustomerSelectControl()
        {
            FormatItem = FormatCustomer;
        }

        public event EventHandler SelectedCustomerChanged
        {
            add => SelectedItemChanged += value;
            remove => SelectedItemChanged -= value;
        }

        public Customer SelectedCustomer => SelectedItem;

        public void BindSearch(Func<string, System.Collections.Generic.IEnumerable<Customer>> search, Func<Customer, string, bool> matches = null)
        {
            Bind(search, matches);
        }

        public bool TrySelectHighlightedCustomer() => TrySelectHighlightedItem();

        private static string FormatCustomer(Customer customer)
        {
            if (customer == null)
                return string.Empty;

            return string.IsNullOrWhiteSpace(customer.Phone)
                ? customer.Name
                : $"{customer.Name} — {customer.Phone}";
        }
    }
}
