namespace Jens.AspNetCore.AutoAPI.Abstractions;
public class Pagination 
{
    public int Page { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public int Total { get; set; } = 100;
}

