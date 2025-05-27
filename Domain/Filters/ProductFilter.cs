namespace Domain.Filters;

public class ProductFilter
{
    public string? Name { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }
    public int? CategoryId { get; set; }
    public int? UserId { get; set; }
    public bool? IsTop { get; set; }
    public bool? IsPremium { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
