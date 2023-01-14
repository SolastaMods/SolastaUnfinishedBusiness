using JetBrains.Annotations;
#if DEBUG
using SolastaUnfinishedBusiness.Api.Diagnostics;
#endif

namespace SolastaUnfinishedBusiness.Api;

internal static partial class DatabaseHelper
{
    [NotNull]
    internal static T GetDefinition<T>(string key) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

#if DEBUG
        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"{typeof(T).Name} not found.");
        }

        if (!db.TryGetElement(key, out var definition))
        {
            throw new SolastaUnfinishedBusinessException(
                $"{key} not found in database {typeof(T).Name}.");
        }

        return definition;
#else
        return db.GetElement(key);
#endif
    }

    internal static bool TryGetDefinition<T>(string key, out T definition) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

#if DEBUG
        if (key != null && db != null)
        {
            return db.TryGetElement(key, out definition);
        }

        definition = null;

        return false;
#else
        return db.TryGetElement(key, out definition);
#endif
    }
}
