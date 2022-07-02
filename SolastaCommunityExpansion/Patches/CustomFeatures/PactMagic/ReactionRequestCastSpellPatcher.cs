using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic;

//Removes low-level sub-option for spell reactions if caster is not-multiclassed warlock
[HarmonyPatch(typeof(ReactionRequestCastSpell), "BuildSlotSubOptions")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class ReactionRequestCastSpell_BuildSlotSubOptions
{
    public static bool Prefix(ReactionRequestCastSpell __instance)
    {
        if (__instance.Character.RulesetCharacter is not RulesetCharacterHero hero)
        {
            return true;
        }

        if (__instance.ReactionParams.RulesetEffect is not RulesetEffectSpell rulesetEffect)
        {
            return true;
        }

        __instance.SubOptionsAvailability.Clear();

        var spellRepertoire = rulesetEffect.SpellRepertoire;
        var minSpellLevel = rulesetEffect.SpellDefinition.SpellLevel;
        var selected = MulticlassGameUiContext
            .AddAvailableSubLevels(__instance.SubOptionsAvailability, hero, spellRepertoire, minSpellLevel);

        if (selected >= 0)
        {
            __instance.SelectSubOption(selected);
        }

        return false;
    }
}
