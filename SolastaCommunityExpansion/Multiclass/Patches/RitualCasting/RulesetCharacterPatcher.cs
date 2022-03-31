using System.Collections.Generic;
using HarmonyLib;
using SolastaCommunityExpansion;

namespace SolastaMulticlass.Patches.RitualCasting
{
    internal static class RulesetCharacterPatcher
    {
        // ensures ritual spells work correctly when MC
        [HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
        internal static class RulesetCharacterCanCastAnyRitualSpell
        {
            internal static bool Prefix(RulesetCharacter __instance, ref bool __result, List<SpellDefinition> ___usableSpells)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return true;
                }

                var canCast = false;

                __instance.EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(__instance.FeaturesToBrowse);

                foreach (var featureDefinition in __instance.FeaturesToBrowse)
                {
                    if (featureDefinition is FeatureDefinitionMagicAffinity featureDefinitionMagicAffinity && featureDefinitionMagicAffinity.RitualCasting != RuleDefinitions.RitualCasting.None)
                    {
                        var ritualType = featureDefinitionMagicAffinity.RitualCasting;

                        __instance.EnumerateUsableRitualSpells(ritualType, ___usableSpells);

                        if (___usableSpells.Count > 0)
                        {
                            canCast = true;
                            break;
                        }
                    }
                }

                __result = canCast;

                return false;
            }
        }
    }
}
