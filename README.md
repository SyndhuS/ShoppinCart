ShoppingCart (example)

Overview
- Small shopping cart library demonstrating DI and testability.
- Targets .NET 10, C# 14.0.

How to run
- Restore and run tests: `dotnet test` from the solution folder.

Key design changes made
- Introduced `IPricingService` for pricing adjustments.
  - `DefaultPricingService` is a no-op (returns base unit price).
- Introduced `ITaxCalculator` with signature `decimal CalculateTax(decimal subtotal, decimal totalBeforeTax)`.
  - `DefaultTaxCalculator` applies a 12.5% tax and rounds to 2 decimal places (MidpointRounding.AwayFromZero).
- `ShoppingCart` now accepts optional `IPricingService` and `ITaxCalculator` dependencies (constructor injection). Defaults are provided so existing code/tests work without changes.

Tests added
- `AddAsync_InvalidArguments_Throws` — validates argument checks for `AddAsync` (quantity and product validation).
- `TaxCalculator_Throws_IsPropagated` — ensures exceptions thrown by the tax calculator are propagated by `ShoppingCart`.

Assumptions
- `IPriceClient.GetPriceAsync` returns a non-negative price for valid products. The cart validates quantity and product but does not enforce price positivity; this can be added if required.
- Tax is calculated from the provided subtotal by default; `CalculateTax` also receives `totalBeforeTax` to give implementations additional context if needed.

Trade-offs and rationale
- Introducing `IPricingService` and `ITaxCalculator` increases flexibility and follows SOLID (OCP, DIP) at the cost of extra indirection and slightly more types to maintain.
- An alternative considered was using a delegate (`Func<decimal, decimal>`) for tax calculation — an interface was chosen for clarity and discoverability, and to allow richer implementations.
- Current `ShoppingCart` is not thread-safe; synchronization would be required for concurrent usage.
