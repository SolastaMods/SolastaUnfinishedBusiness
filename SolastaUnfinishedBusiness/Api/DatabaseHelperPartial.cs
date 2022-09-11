using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;

namespace SolastaUnfinishedBusiness.Api;

public static partial class DatabaseHelper
{
    [NotNull]
    public static T GetDefinition<T>(string key, string guid) where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (db == null)
        {
            throw new SolastaUnfinishedBusinessException($"Database of type {typeof(T).Name} not found.");
        }

        var definition = db.TryGetElement(key, guid);

        if (definition == null)
        {
            throw new SolastaUnfinishedBusinessException(
                $"Definition with name={key} or guid={guid} not found in database {typeof(T).Name}.");
        }

        return definition;
    }

    public static bool TryGetDefinition<T>([CanBeNull] string key, string guid, [CanBeNull] out T definition)
        where T : BaseDefinition
    {
        var db = DatabaseRepository.GetDatabase<T>();

        if (key == null || db == null)
        {
            definition = null;
            return false;
        }

        definition = db.TryGetElement(key, guid);

        return definition != null;
    }
}
