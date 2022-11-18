using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Diagnostics;

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

    internal static class GadgetBlueprints
    {
        internal static GadgetBlueprint TeleporterIndividual { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterIndividual");

        internal static GadgetBlueprint TeleporterParty { get; } =
            GetDefinition<GadgetBlueprint>("TeleporterParty");

        internal static GadgetBlueprint VirtualExit { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExit");

        internal static GadgetBlueprint VirtualExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("VirtualExitMultiple");

        internal static GadgetBlueprint Exit { get; } =
            GetDefinition<GadgetBlueprint>("Exit");

        internal static GadgetBlueprint ExitMultiple { get; } =
            GetDefinition<GadgetBlueprint>("ExitMultiple");
    }

    internal static class GadgetDefinitions
    {
        internal static GadgetDefinition Activator { get; } =
            GetDefinition<GadgetDefinition>("Activator");
    }
}
