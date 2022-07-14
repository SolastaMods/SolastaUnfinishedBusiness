using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using TA;

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
        var rulesetCharacter = Global.ActivePlayerCharacter.RulesetCharacter;

        rulesetCharacter.EnumerateFeaturesToBrowse<IOnCharacterKill>(rulesetCharacter.FeaturesToBrowse);

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (IOnCharacterKill characterKill in rulesetCharacter.FeaturesToBrowse)
        {
            characterKill.OnCharacterKill(character, dropLoot, removeBody, forceRemove, considerDead, becomesDying);
        }
    }
}
