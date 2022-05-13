using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.Multiclass.RitualCasting
{
    // ensures ritual spells work correctly when MC
    [HarmonyPatch(typeof(RulesetCharacter), "CanCastAnyRitualSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_CanCastAnyRitualSpell
    {
        internal static bool Prefix(RulesetCharacter __instance, ref bool __result, List<SpellDefinition> ___usableSpells)
        {
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
