using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class FeatureDefinitionCastSpellPatcher
    {
        [HarmonyPatch(typeof(FeatureDefinitionCastSpell), "ComputeHighestSpellLevel")]
        internal static class FeatureDefinitionCastSpellComputeHighestSpellLevel
        {
            internal static void Postfix(ref int __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (Models.LevelUpContext.LevelingUp && Models.LevelUpContext.IsMulticlass)
                {
                    __result = Models.SharedSpellsContext.GetClassSpellLevel(
                        Models.LevelUpContext.SelectedHero, 
                        Models.LevelUpContext.SelectedClass, 
                        Models.LevelUpContext.SelectedSubclass);
                }
            }
        }
    }
}
