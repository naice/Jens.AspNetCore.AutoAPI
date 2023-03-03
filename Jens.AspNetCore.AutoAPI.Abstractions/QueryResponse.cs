namespace Jens.AspNetCore.AutoAPI.Abstractions;

public interface IQueryResponse<T> : IQueryResponse
{
    List<T> Data { get; set; }
}

public interface IQueryResponse
{
    public Sorting Sorting { get; set; }
    public Pagination Pagination { get; set; } 
    public string? Filter { get; set; }
}

public class QueryResponse<T> : BaseResponse, IQueryResponse<T>
{
    public List<T> Data { get; set; } = new List<T>();
    public Sorting Sorting { get; set; } = new Sorting();
    public Pagination Pagination { get; set; } = new Pagination();
    public string? Filter { get; set; }
}

