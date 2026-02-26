namespace ShoppingCart
{
    public class ShoppingCart
    {
        private readonly IPriceClient _priceClient;
        private readonly Dictionary<string, CartItem> _items = new(StringComparer.OrdinalIgnoreCase);
        private const decimal TaxRate = 0.125m; // 12.5%

        public ShoppingCart(IPriceClient priceClient)
        {
            _priceClient = priceClient ?? throw new ArgumentNullException(nameof(priceClient));
        }
        public IReadOnlyCollection<CartItem> Items => _items.Values.ToList().AsReadOnly();
        public async Task AddAsync(string product, int quantity = 1)
        {
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
            if (string.IsNullOrWhiteSpace(product)) throw new ArgumentNullException(nameof(product));

            var unitPrice = await _priceClient.GetPriceAsync(product).ConfigureAwait(false);

            if (_items.TryGetValue(product, out var existing))
            {
                existing.Quantity += quantity;
            }
            else
            {
                _items[product] = new CartItem(product, quantity, unitPrice);
            }
        }
        public decimal Subtotal => _items.Values.Sum(item => item.TotalPrice);
        public decimal Tax => Math.Round(Subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
        public decimal Total => Subtotal + Tax;
    }
}
