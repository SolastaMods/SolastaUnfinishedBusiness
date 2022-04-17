using HarmonyLib;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageDeitySelectionPanelPatcher
    {
        // disables the deity selection screen if any classes multiclass into a Cleric or if any classes except Cleric multiclasses into a Paladin
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageDeitySelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageDeitySelectionPanel __instance, RulesetCharacterHero ___currentHero)
            {
                if (LevelUpContext.IsLevelingUp(___currentHero))
                {
                    __instance.SetField("isRelevant", LevelUpContext.RequiresDeity(___currentHero));
                }
            }
        }
    }
}
