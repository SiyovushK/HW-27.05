namespace Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool IsTop { get; set; } = false;
    public bool IsPremium { get; set; } = false;
    public DateTime? PremiumOrTopExpiryDate { get; set; }
    
    public virtual Category Category { get; set; }
    public virtual User User { get; set; }
    public virtual List<Order> Orders { get; set; } = new();
}