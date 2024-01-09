using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Models;
using TA;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationCharacterManagerPatcher
{
    //PATCH: recalculates additional party members positions (PARTYSIZE)
    [HarmonyPatch(typeof(GameLocationCharacterManager),
        nameof(GameLocationCharacterManager.UnlockCharactersForLoading))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class UnlockCharactersForLoading_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] GameLocationCharacterManager __instance)
        {
            var partyCharacters = __instance.PartyCharacters;

            for (var idx = ToolsContext.GamePartySize; idx < partyCharacters.Count; idx++)
            {
                var position = partyCharacters[idx % ToolsContext.GamePartySize].LocationPosition;

                partyCharacters[idx].LocationPosition = new int3(position.x, position.y, position.z);
            }
        }
    }


    [HarmonyPatch(typeof(GameLocationCharacterManager), nameof(GameLocationCharacterManager.LoseWildShapeAndRefund))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LoseWildShapeAndRefund_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] GameLocationCharacterManager __instance, ref bool __result,
            GameLocationCharacter character)
        {
            //PATCH: fixes crashes on characters affected by Shapechange (True Polymorph) or similar
            var rulesetCharacter = character.RulesetCharacter;

            if (!rulesetCharacter.HasConditionOfType(ConditionWildShapeSubstituteForm))
            {
                //not shape shifted - use default method
                return true;
            }

            var power = rulesetCharacter.OriginalFormCharacter
                ?.GetPowerFromDefinition(DatabaseHelper.FeatureDefinitionPowers.PowerDruidWildShape);
            if (power != null)
            {
                //has wildshape power - use default method
                return true;
            }

            //no wildshape power - kill shape shifted form and skip default method
            __instance.KillCharacter(character, false, true, true, true, false);
            __result = true;

            return false;
        }
    }
}
