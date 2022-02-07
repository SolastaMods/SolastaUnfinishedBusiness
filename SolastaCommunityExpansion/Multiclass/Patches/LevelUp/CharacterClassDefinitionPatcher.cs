using System.Collections.Generic;
using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterClassDefinitionPatcher
    {
        // returns a filtered features unlocks list depending on level up context
        [HarmonyPatch(typeof(CharacterClassDefinition), "FeatureUnlocks", MethodType.Getter)]
        internal static class CharacterClassDefinitionFeatureUnlocks
        {
            internal static void Postfix(ref List<FeatureUnlockByLevel> __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    __result = Models.LevelUpContext.FilteredFeaturesUnlocks(__result);
                }
            }
        }
    }
}
