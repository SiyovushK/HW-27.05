namespace Domain.DTOs.Product;

public class PromoteProductDto
{
    public bool SetTop { get; set; }
    public bool SetPremium { get; set; }
    public int DurationInDays { get; set; } = 7;
}