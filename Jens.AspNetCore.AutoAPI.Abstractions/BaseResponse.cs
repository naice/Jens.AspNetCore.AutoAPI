namespace Jens.AspNetCore.AutoAPI.Abstractions;

public class BaseResponse : IBaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

