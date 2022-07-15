using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Insertion;

[HarmonyPatch(typeof(GameLocationCharacterManager), "KillCharacter")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationCharacterManager_KillCharacter
{
    internal static void Prefix(
        GameLocationCharacter character,
        bool dropLoot,
        bool removeBody,
        bool forceRemove,
        bool considerDead,
        bool becomesDying)
    {
        var rulesetCharacter = Global.ActivePlayerCharacter?.RulesetCharacter;

        if (rulesetCharacter == null)
        {
            return;
        }

        var features = new List<FeatureDefinition>();

        rulesetCharacter.EnumerateFeaturesToBrowse<IOnCharacterKill>(features);

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (IOnCharacterKill characterKill in features)
        {
            characterKill.OnCharacterKill(character, dropLoot, removeBody, forceRemove, considerDead, becomesDying);
        }
    }
}
