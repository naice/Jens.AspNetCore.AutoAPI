namespace Jens.AspNetCore.AutoAPI;

public static class ObjectExtensions
{
    public static object CloneInto(this object obj, Type targetType)
    {
        // TODO: consider performance..
        var s = System.Text.Json.JsonSerializer.Serialize(obj);
        var target = System.Text.Json.JsonSerializer.Deserialize(s, targetType);

        if (target == null) 
            throw new ArgumentException(nameof(obj));

        return target;
    }
}