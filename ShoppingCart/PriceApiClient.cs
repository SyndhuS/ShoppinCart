using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.IO;

namespace ShoppingCart
{
    public class PriceApiClient : IPriceClient
    {
        private const string BaseUrl = "https://equalexperts.github.io/backend-take-home-test-data";
        private readonly HttpClient _httpClient;
        public PriceApiClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<decimal> GetPriceAsync(string product)
        {
            if (string.IsNullOrWhiteSpace(product)) throw new ArgumentNullException(nameof(product));
            var url = $"{BaseUrl}/{Uri.EscapeDataString(product)}.json";
            using var resp = await _httpClient.GetAsync(url).ConfigureAwait(false);
            resp.EnsureSuccessStatusCode();
            using var stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var doc = await JsonDocument.ParseAsync(stream).ConfigureAwait(false);
            var price = FindFirstNumber(doc.RootElement);
            if (price is null) throw new InvalidOperationException("Price not found in API response.");
            return price.Value;
        }
        private static decimal? FindFirstNumber(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in el.EnumerateObject())
                    {
                        var found = FindFirstNumber(prop.Value);
                        if (found is not null) return found;
                    }
                    break;
                case JsonValueKind.Array:
                    foreach (var item in el.EnumerateArray())
                    {
                        var found = FindFirstNumber(item);
                        if (found is not null) return found;
                    }
                    break;
                case JsonValueKind.Number:
                    if (el.TryGetDecimal(out var d)) return d;
                    if (el.TryGetDouble(out var dd)) return (decimal)dd;
                    break;
                case JsonValueKind.String:
                    if (decimal.TryParse(el.GetString(), out var parsed)) return parsed;
                    break;
            }
            return null;
        }
    }
}
