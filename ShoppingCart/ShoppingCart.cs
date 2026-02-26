using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart
{
    public class ShoppingCart
    {
        private readonly IPriceClient _priceClient;
        private readonly IPricingService _pricingService;
        private readonly ITaxCalculator _taxCalculator;
        private readonly Dictionary<string, CartItem> _items = new(StringComparer.OrdinalIgnoreCase);

        public ShoppingCart(IPriceClient priceClient, IPricingService? pricingService = null, ITaxCalculator? taxCalculator = null)
        {
            _priceClient = priceClient ?? throw new ArgumentNullException(nameof(priceClient));
            _pricingService = pricingService ?? new DefaultPricingService();
            _taxCalculator = taxCalculator ?? new DefaultTaxCalculator();
        }

        public IReadOnlyCollection<CartItem> Items => _items.Values.ToList().AsReadOnly();

        public async Task AddAsync(string product, int quantity = 1)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (string.IsNullOrWhiteSpace(product)) throw new ArgumentNullException(nameof(product));

            var unitPrice = await _priceClient.GetPriceAsync(product).ConfigureAwait(false);
            var adjustedUnitPrice = _pricingService.GetUnitPrice(unitPrice, product, quantity);

            if (_items.TryGetValue(product, out var existing))
            {
                existing.Quantity += quantity;
            }
            else
            {
                _items[product] = new CartItem(product, quantity, adjustedUnitPrice);
            }
        }

        public decimal Subtotal => _items.Values.Sum(item => item.TotalPrice);
        public decimal Tax => _taxCalculator.CalculateTax(Subtotal, Subtotal);
        public decimal Total => Subtotal + Tax;
    }
}
