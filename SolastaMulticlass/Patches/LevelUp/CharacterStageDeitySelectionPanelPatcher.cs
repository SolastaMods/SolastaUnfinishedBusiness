using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageDeitySelectionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageDeitySelectionPanelUpdateRelevance
        {
            internal static void Postfix(RulesetCharacterHero ___currentHero, ref bool ___isRelevant)
            {
                if (LevelUpContext.IsLevelingUp(___currentHero))
                {
                    ___isRelevant = LevelUpContext.RequiresDeity(___currentHero);
                }
            }
        }
    }
}
