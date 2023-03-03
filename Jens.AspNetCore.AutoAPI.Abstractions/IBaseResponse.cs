namespace Jens.AspNetCore.AutoAPI.Abstractions;

public interface IBaseResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

