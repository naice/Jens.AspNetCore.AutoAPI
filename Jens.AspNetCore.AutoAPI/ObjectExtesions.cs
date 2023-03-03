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

    public static Target CloneInto<Target>(this object obj) where Target : class
    {
        if (CloneInto(obj, typeof(Target)) is not Target result)
            throw new ArgumentException(nameof(obj));

        return result;
    }
}