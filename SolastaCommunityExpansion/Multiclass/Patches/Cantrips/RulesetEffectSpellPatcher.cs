using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.Cantrips
{
    // these patches enforces cantrips to be cast at character level
    internal static class RulesetEffectSpellPatcher
    {
        internal static int CasterLevel { get; private set; }

        // use this patch to determine the spell context and set the CasterLevel to correctly set the caster level on other 4 methods traps
        [HarmonyPatch(typeof(RulesetEffectSpell), "ComputeTargetParameter")]
        internal static class RulesetEffectSpellComputeTargetParameter
        {
            internal static void Prefix(RulesetEffectSpell __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                CasterLevel = 0;

                if (__instance?.Caster is not RulesetCharacterHero heroWithSpellRepertoire)
                {
                    return;
                }

                var spellRepertoire = __instance.SpellRepertoire;
                var spellDefinition = __instance.SpellDefinition;

                if (spellDefinition != null && spellRepertoire?.KnownCantrips.Contains(spellDefinition) == true)
                {
                    CasterLevel = heroWithSpellRepertoire.ClassesHistory.Count;
                }
            }
        }

        [HarmonyPatch(typeof(RulesetEffectSpell), "GetClassLevel")]
        internal static class RulesetEffectSpellGetClassLevel
        {
            internal static void Postfix(ref int __result)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                if (CasterLevel > 0)
                {
                    __result = CasterLevel;
                }
            }
        }
    }
}
