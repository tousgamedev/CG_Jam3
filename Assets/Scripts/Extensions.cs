using System.Collections.Generic;
using UnityEngine;

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
    
    #region Transforms/Vectors

    public static Vector3 DirectionTo(this Transform startTransform, Transform targetTransform)
    {
        return targetTransform.position - startTransform.position;
    }
    
    public static Vector3 DirectionTo(this Transform startTransform, Vector3 targetPosition)
    {
        return targetPosition - startTransform.position;
    }
    
    public static Vector3 DirectionTo(this Vector3 startVector, Vector3 targetVector)
    {
        return targetVector - startVector;
    }

    public static Vector2 DirectionTo(this Vector2 startVector, Vector2 targetVector)
    {
        return targetVector - startVector;
    }
    
    #endregion
}