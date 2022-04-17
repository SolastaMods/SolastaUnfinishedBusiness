using HarmonyLib;
using SolastaModApi.Infrastructure;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSubclassSelectionPanelPatcher
    {
        // disables the sub class selection screen if the deity screen was enabled
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageSubclassSelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageSubclassSelectionPanel __instance, RulesetCharacterHero ___currentHero)
            {
                if (LevelUpContext.IsLevelingUp(___currentHero) && LevelUpContext.RequiresDeity(___currentHero))
                {
                    __instance.SetField("isRelevant", false);
                }
            }
        }
    }
}
