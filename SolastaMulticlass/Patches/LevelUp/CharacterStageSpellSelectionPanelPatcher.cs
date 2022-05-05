using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSpellSelectionPanelPatcher
    {
        // patches the method to get my own class and level for level up
        [HarmonyPatch(typeof(CharacterStageSpellSelectionPanel), "EnterStage")]
        internal static class CharacterStageSpellSelectionPanelEnterStage
        {
            public static void Prefix(RulesetCharacterHero ___currentHero)
            {
                LevelUpContext.CacheAllowedSpells(___currentHero);
            }
        }
    }
}
