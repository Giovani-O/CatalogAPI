namespace CatalogAPI.Pagination
{
    public class ProductsPriceFilter : QueryStringParameters
    {
        public decimal? Price { get; set; }
        public string? PriceCondition { get; set; }
    }
}
