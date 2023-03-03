namespace Jens.AspNetCore.AutoAPI.Abstractions;

public interface IQueryRequest : IQueryResponse {}

public class QueryRequest : IQueryRequest
{
    public Sorting Sorting { get; set; } = new Sorting();
    public Pagination Pagination { get; set; } = new Pagination();
    public string? Filter { get; set; }
}
