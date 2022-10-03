using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    [NotNull]
    private static T GetDefinition<T>(string key) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException($"Database of type {typeof(T).Name} not found.");
        }

        if (!db.TryGetElement(key, out var definition))
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    internal static bool TryGetDefinition<T>(string key, out T definition)
        where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key != null && db != null)
        {
            return db.TryGetElement(key, out definition);
        }

        definition = null;

        return false;
    }
}
