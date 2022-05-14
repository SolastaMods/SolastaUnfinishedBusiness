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

                __instance.SubOptionsAvailability.Clear();
                var spellRepertoire = rulesetEffect.SpellRepertoire;
                var minSpellLebvel = rulesetEffect.SpellDefinition.SpellLevel;

                var selected = MulticlassGameUiContext.AddAvailableSubLevels(__instance.SubOptionsAvailability, hero, spellRepertoire, minSpellLebvel);
                if (selected >= 0)
                {
                    __instance.SelectSubOption(selected);
                }

                return false;
            }
        }
    }
}
