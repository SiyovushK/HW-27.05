using Domain.Enums;

namespace Domain.Filters;

public class OrderFilter
{
    public int? UserId { get; set; }
    public int? ProductId { get; set; }
    public Status? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}