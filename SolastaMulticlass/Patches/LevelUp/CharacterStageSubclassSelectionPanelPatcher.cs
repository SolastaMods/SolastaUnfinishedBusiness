using HarmonyLib;
using SolastaMulticlass.Models;

namespace SolastaMulticlass.Patches.LevelUp
{
    internal static class CharacterStageSubclassSelectionPanelPatcher
    {
        [HarmonyPatch(typeof(CharacterStageSubclassSelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageSubclassSelectionPanelUpdateRelevance
        {
            internal static void Postfix(RulesetCharacterHero ___currentHero, ref bool ___isRelevant)
            {
                if (LevelUpContext.IsLevelingUp(___currentHero) && LevelUpContext.RequiresDeity(___currentHero))
                {
                    ___isRelevant = false;
                }
            }
        }
    }
}
