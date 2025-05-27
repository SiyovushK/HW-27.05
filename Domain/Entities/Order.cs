using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public Status Status { get; set; }

    public virtual User User { get; set; }
    public virtual Product Products { get; set; }
}