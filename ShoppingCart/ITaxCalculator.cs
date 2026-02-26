using System;

namespace ShoppingCart
{
    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal subtotal, decimal totalBeforeTax);
    }

    public sealed class DefaultTaxCalculator : ITaxCalculator
    {
        private const decimal TaxRate = 0.125m; 
        public decimal CalculateTax(decimal subtotal, decimal totalBeforeTax)
        {
            return Math.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
        }
    }
}
