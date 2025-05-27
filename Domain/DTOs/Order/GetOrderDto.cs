using Domain.Enums;

namespace Domain.DTOs.Order;

public class GetOrderDto
{
    public int Id { get; set; } 
    public int UserId { get; set; } 
    public int ProductId { get; set; } 
    public int Quantity { get; set; } 
    public DateTime OrderDate { get; set; } 
    public Status Status { get; set; } 
}
