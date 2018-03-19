using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blockchain.rate.service.Domain
{
    public enum InvoiceStatus
    {
        New,
        Closed
    };

    public class Invoice
    {
        public string Number { get; protected set; }
        public string Date { get; protected set; }

        public Item Item { get; protected set; }
        public decimal Quantity { get; set; }

        public User User { get; set; }
        public IReadOnlyCollection<User> Handlers { get; set; }

        public InvoiceStatus Status { get; set; }
    }

    public class Item
    {
        public string Name { get; protected set; }
    }

    public class User
    {
        public string Email { get; protected set; }
    }
}
