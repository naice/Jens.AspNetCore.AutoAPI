namespace Jens.AspNetCore.AutoAPI.Abstractions;

public class Sorting
{
    public SortingDirection Direction { get; set; } = SortingDirection.ASCENDING;
    public string Field { get; set; } = string.Empty;
}

