using HarmonyLib;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterStageDeitySelectionPanelPatcher
    {
        // disables the deity selection screen if any classes multiclass into a Cleric or if any classes except Cleric multiclasses into a Paladin
        [HarmonyPatch(typeof(CharacterStageDeitySelectionPanel), "UpdateRelevance")]
        internal static class CharacterStageDeitySelectionPanelUpdateRelevance
        {
            internal static void Postfix(CharacterStageDeitySelectionPanel __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (Models.LevelUpContext.LevelingUp)
                {
                    __instance.SetField("isRelevant", Models.LevelUpContext.RequiresDeity);
                }
            }
        }
    }
}
