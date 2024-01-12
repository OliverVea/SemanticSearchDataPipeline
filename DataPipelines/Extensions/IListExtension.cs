namespace DataPipelines.Extensions;

public static class ListExtension
{
    public static void SetFirst<T>(this IList<T> list, T? value)
    {
        if (value != null)
        {
            if (list.Any()) list[0] = value;
            else list.Add(value);
        }
        else if (list.Any()) list.Clear();
    }
}