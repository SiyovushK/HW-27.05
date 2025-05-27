namespace Domain.Filters;

public class UserFilter
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
