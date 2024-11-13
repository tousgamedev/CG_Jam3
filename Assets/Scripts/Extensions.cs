using System.Collections.Generic;

public static class Extensions
{
    #region Collections

    public static T GetAtIndexOrLast<T>(this IReadOnlyList<T> collection, int index, T defaultValue = default)
    {
        if (collection == null)
        {
            return defaultValue;
        }

        if (index < 0 || index >= collection.Count)
        {
            return defaultValue;
        }

        return collection[index];
    }

    public static void IncrementIndex<T>(this IReadOnlyList<T> collection, ref int index)
    {
        if (collection == null || index < 0)
        {
            index = -1;
            return;
        }

        if (index + 1 < collection.Count)
        {
            index++;
            return;
        }

        index = collection.Count - 1;
    }

    public static bool TryGetValue<T>(this IReadOnlyList<T> collection, int index, out T value)
    {
        value = default;
        if (collection == null)
        {
            return false;
        }

        if (index < 0 || index >= collection.Count)
        {
            return false;
        }

        value = collection[index];
        return true;
    }

    #endregion
}