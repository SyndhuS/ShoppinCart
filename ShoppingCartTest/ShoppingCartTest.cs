using ShoppingCart;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCartTest 
{
    public class ShoppingCartTest
    {

        public class TestPriceClient : IPriceClient
        {               
            public Task<decimal> GetPriceAsync(string product)
            {
                return product.ToLowerInvariant() switch
                {
                    "cornflakes" => Task.FromResult(2.52m),
                    "weetabix" => Task.FromResult(9.98m),
                    _ => Task.FromResult(1.00m)
                };
            }
        }
        public class ThrowingTaxCalculator : ITaxCalculator
        {
            public decimal CalculateTax(decimal subtotal, decimal totalBeforeTax)
            {
                throw new InvalidOperationException("tax service failed");
            }
        }
        [Test]
        public async Task SampleScenario_ComputesExpectedTotals()
        {
            var client = new TestPriceClient();
            var cart = new ShoppingCart.ShoppingCart(client);

            await cart.AddAsync("cornflakes", 1);
            await cart.AddAsync("cornflakes", 1);
            await cart.AddAsync("weetabix", 1);

            // quantities
            Assert.AreEqual(2, cart.Items.Count);
            var items = cart.Items.ToList();
            Assert.AreEqual(2, items[0].Quantity);
            Assert.AreEqual(1, items[1].Quantity);

            Assert.AreEqual(15.02m, cart.Subtotal);
            Assert.AreEqual(1.88m, cart.Tax);
            Assert.AreEqual(16.90m, cart.Total);
        }
        [Test]
        public async Task RoundsUp_TwoDecimalPlaces()
        {
            var client = new TestPriceClient();
            var cart = new ShoppingCart.ShoppingCart(client);
            await cart.AddAsync("cornflakes", 1); // 2.52
            await cart.AddAsync("cornflakes", 1); // 2.52 -> subtotal 5.04

            Assert.AreEqual(5.04m, cart.Subtotal);
            Assert.AreEqual(0.63m, cart.Tax); // 5.04 * 0.125 = 0.63 exactly
            Assert.AreEqual(5.67m, cart.Total);
        }

        [Test]
        public async Task AddAsync_InvalidArguments_Throws()
        {
            var client = new TestPriceClient();
            var cart = new ShoppingCart.ShoppingCart(client);

            // invalid quantities
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cart.AddAsync("cornflakes", 0));
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await cart.AddAsync("cornflakes", -1));

            // null or whitespace product
            Assert.ThrowsAsync<ArgumentNullException>(async () => await cart.AddAsync(null!, 1));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await cart.AddAsync("", 1));
            Assert.ThrowsAsync<ArgumentNullException>(async () => await cart.AddAsync("   ", 1));
        }

        [Test]
        public async Task TaxCalculator_Throws_IsPropagated()
        {
            var client = new TestPriceClient();
            var taxCalc = new ThrowingTaxCalculator();
            var cart = new ShoppingCart.ShoppingCart(client, null, taxCalc);

            await cart.AddAsync("cornflakes", 1);

            Assert.Throws<InvalidOperationException>(() => { var _ = cart.Tax; });
            Assert.Throws<InvalidOperationException>(() => { var _ = cart.Total; });
        }
    }
}
