using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // Don't present the upcast menu on SC Warlock
    [HarmonyPatch(typeof(SpellActivationBox), "BindSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellActivationBox_Bind
    {
        public static bool HasAdditionalSlotAdvancement(
            EffectDescription effectDescription,
            RulesetCharacter caster,
            RulesetSpellRepertoire spellRepertoire)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);

            if (isWarlockSpell)
            {
                var hero = caster as RulesetCharacterHero;
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(hero);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

                return sharedSpellLevel > warlockSpellLevel;
            }

            return effectDescription.HasAdditionalSlotAdvancement;
        }

        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var hasAdditionalSlotAdvancementMethod = typeof(EffectDescription).GetMethod("get_HasAdditionalSlotAdvancement");
            var myHasAdditionalSlotAdvancementMethod = typeof(SpellActivationBox_Bind).GetMethod("HasAdditionalSlotAdvancement");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(hasAdditionalSlotAdvancementMethod))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1); // caster
                    yield return new CodeInstruction(OpCodes.Ldarg_2); // spellRepertoire
                    yield return new CodeInstruction(OpCodes.Call, myHasAdditionalSlotAdvancementMethod);
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
