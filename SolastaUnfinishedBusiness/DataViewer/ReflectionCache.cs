using System.Collections;
using System.Reflection;

namespace SolastaUnfinishedBusiness.DataViewer;

internal static partial class ReflectionCache
{
    private const BindingFlags AllFlags =
        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

    private const int SizeLimit = 1000;

    private static readonly Queue Cache = new();

    private static void EnqueueCache(object obj)
    {
        while (Cache.Count >= SizeLimit)
        {
            Cache.Dequeue();
        }

        Cache.Enqueue(obj);
    }
}
