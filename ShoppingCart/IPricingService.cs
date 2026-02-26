using System;

namespace ShoppingCart
{
    public interface IPricingService
    {
        decimal GetUnitPrice(decimal baseUnitPrice, string product, int quantity);
    }

    public sealed class DefaultPricingService : IPricingService
    {
        public decimal GetUnitPrice(decimal baseUnitPrice, string product, int quantity) => baseUnitPrice;
    }
}
