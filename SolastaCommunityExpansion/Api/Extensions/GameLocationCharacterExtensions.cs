using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.Api.Extensions;

public static class GameLocationCharacterExtensions
{
    private static readonly Dictionary<string, int> SkipAttackModes = new();

    [NotNull]
    private static string Key([NotNull] GameLocationCharacter instance)
    {
        return $"{instance.Name}:{instance.Guid}";
    }

    public static void SetSkipAttackModes(this GameLocationCharacter instance, int skip)
    {
        SkipAttackModes.AddOrReplace(Key(instance), skip);
    }

    public static void RemoveSkipAttackModes(this GameLocationCharacter instance)
    {
        SkipAttackModes.Remove(Key(instance));
    }

    public static int GetSkipAttackModes(this GameLocationCharacter instance)
    {
        var key = Key(instance);
        if (SkipAttackModes.ContainsKey(key))
        {
            return SkipAttackModes[key];
        }

        return 0;
    }
}
