using Domain.Enums;

namespace Domain.DTOs.Order;

public class UpdateOrderDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } 
    public Status Status { get; set; }
}
