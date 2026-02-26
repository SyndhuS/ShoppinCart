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
    }
}
