using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    
    //Removes low-level sub-option for spell reactions if caster is not-multiclassed warlock
    internal static class ReactionRequestCastSpellPatcher
    {
        [HarmonyPatch(typeof(ReactionRequestCastSpell), "BuildSlotSubOptions")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class ReactionRequestCastSpell_BuildSlotSubOptions
        {
            public static bool Prefix(ReactionRequestCastSpell __instance)
            {
                var hero = __instance.Character.RulesetCharacter as RulesetCharacterHero;
                if (hero == null) { return true; }

                var rulesetEffect = __instance.ReactionParams.RulesetEffect as RulesetEffectSpell;
                if (rulesetEffect == null) { return true; }
                
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
                var isMulticaster = SharedSpellsContext.IsMulticaster(hero);
                var hasPactMagic = warlockSpellLevel > 0;

                __instance.SubOptionsAvailability.Clear();
                var spellRepertoire = rulesetEffect.SpellRepertoire;
                var minSpellLebvel = rulesetEffect.SpellDefinition.SpellLevel;
                var maxRepertoireLevel = spellRepertoire.MaxSpellLevelOfSpellCastingLevel;
                var maxSpellLevel = Math.Max(maxRepertoireLevel, warlockSpellLevel);
                var selected = false;

                for (int level = minSpellLebvel; level <= maxSpellLevel; ++level)
                {
                    spellRepertoire.GetSlotsNumber(level, out var remaining, out var max);
                    if (max > 0 && (
                            level <= maxRepertoireLevel
                            && (isMulticaster || !hasPactMagic)
                            || level == warlockSpellLevel
                        ))
                    {
                        __instance.SubOptionsAvailability.Add(level, remaining > 0);
                        if (!selected && remaining > 0)
                        {
                            selected = true;
                            __instance.SelectSubOption(level - minSpellLebvel);
                        }
                    }
                }

                return false;
            }
        }
    }
}
