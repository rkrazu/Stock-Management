using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Stock_Managemnet.Models
{
    [DataContract]
    public class StockData
    {
        [DataMember] public List<Product> Products { get; set; } = new List<Product>();
        [DataMember] public List<Customer> Customers { get; set; } = new List<Customer>();
        [DataMember] public List<StockTransaction> Transactions { get; set; } = new List<StockTransaction>();
        [DataMember] public int NextInvoiceNumber { get; set; } = 1;
    }
}
