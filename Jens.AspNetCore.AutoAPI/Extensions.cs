namespace Jens.AspNetCore.AutoAPI;

public static class Extensions
{
    public static bool ForAll<T>(this IEnumerable<T>? enumerable, Action<T> action)
    {
        if (enumerable == null) return false;
        bool any = false;
        foreach (var item in enumerable)
        {
            action(item);
            any = true;
        }
        return any;
    }
}