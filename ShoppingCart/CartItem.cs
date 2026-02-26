using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart
{
    public sealed class CartItem
    {
        public string Product { get; }
        public int Quantity { get; internal set; }
        public decimal UnitPrice { get; }

        public CartItem(string product, int quantity, decimal unitPrice)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            Product = product ?? throw new ArgumentNullException(nameof(product));
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
