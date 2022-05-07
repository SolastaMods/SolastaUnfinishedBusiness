using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            public static void Postfix(ReactionRequestCastSpell __instance)
            {
                var hero = __instance.Character.RulesetCharacter as RulesetCharacterHero;
                if (hero == null) { return; }

                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
                if (SharedSpellsContext.IsMulticaster(hero) || warlockSpellLevel == 0) { return; }

                var rulesetEffect = __instance.ReactionParams.RulesetEffect as RulesetEffectSpell;
                if (rulesetEffect == null) { return; }

                var minSpellLebvel = rulesetEffect.SpellDefinition.SpellLevel;
                var selected = false;
                var optionsAvailability = __instance.SubOptionsAvailability;
                var levels = optionsAvailability.Keys.ToList();

                foreach (var spellLevel in levels)
                {
                    if (spellLevel != warlockSpellLevel)
                    {
                        optionsAvailability.Remove(spellLevel);
                    }
                    else if (!selected && optionsAvailability[spellLevel])
                    {
                        __instance.SelectSubOption(spellLevel - minSpellLebvel);
                        selected = true;
                    }
                }

                if (!selected)
                {
                    __instance.SelectSubOption(optionsAvailability.Keys.Min() - minSpellLebvel);
                }
            }
        }
    }
}
