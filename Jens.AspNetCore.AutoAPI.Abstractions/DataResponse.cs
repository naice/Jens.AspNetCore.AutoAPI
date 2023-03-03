namespace Jens.AspNetCore.AutoAPI.Abstractions;

public interface IDataResponse<TData> : IBaseResponse 
{
    List<TData> Data { get; set; }
}

public class DataResponse<TData> :  BaseResponse, IDataResponse<TData>
{
    public List<TData> Data { get; set; } = new List<TData>();
}

