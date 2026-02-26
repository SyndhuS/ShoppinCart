using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart
{
    public interface IPriceClient
    {
        Task<decimal> GetPriceAsync(string product);
    }
}
