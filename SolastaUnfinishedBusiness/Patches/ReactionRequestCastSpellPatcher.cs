using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

//PATCH: removes low-level sub-option for spell reactions if caster is not-multiclass warlock (MULTICLASS)
[UsedImplicitly]
public static class ReactionRequestCastSpellPatcher
{
    [HarmonyPatch(typeof(ReactionRequestCastSpell), nameof(ReactionRequestCastSpell.BuildSlotSubOptions))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class BuildSlotSubOptions_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ReactionRequestCastSpell __instance)
        {
            if (__instance.ReactionParams.RulesetEffect is RulesetEffectSpell rulesetEffectSpell)
            {
                // this is a collateral case to support spells from race repertoires
                // but UI for some reason still displays slots from the caster class
                rulesetEffectSpell.spellRepertoire =
                    __instance.Character.RulesetCharacter.SpellRepertoires.FirstOrDefault(x =>
                        x.KnownSpells.Contains(rulesetEffectSpell.SpellDefinition));
            }
        }

        [UsedImplicitly]
        public static void Postfix(ReactionRequestCastSpell __instance)
        {
            if (__instance.Character.RulesetCharacter is not RulesetCharacterHero hero
                || (SharedSpellsContext.GetWarlockSpellRepertoire(hero) != null
                    && !SharedSpellsContext.IsMulticaster(hero)))
            {
                return;
            }

            var optionsAvailability = __instance.SubOptionsAvailability;
            var reactionParams = __instance.ReactionParams;
            var repertoire = reactionParams.SpellRepertoire
                             ?? (reactionParams.RulesetEffect as RulesetEffectSpell)?.SpellRepertoire;

            if (repertoire == null)
            {
                return;
            }

            optionsAvailability.Clear();

            var rulesetEffectSpell = __instance.ReactionParams.RulesetEffect as RulesetEffectSpell;
            var spellLevel = rulesetEffectSpell.SpellDefinition.SpellLevel;
            var selected =
                MulticlassGameUiContext.AddAvailableSubLevels(optionsAvailability, hero, repertoire, spellLevel);

            if (selected >= 0)
            {
                __instance.SelectSubOption(selected);
            }
        }
    }

    [HarmonyPatch(typeof(ReactionRequestCastSpell), nameof(ReactionRequestCastSpell.SelectSubOption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectSubOption_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ReactionRequestCastSpell __instance, int option)
        {
            //this should always be false
            if (__instance.ReactionParams.RulesetEffect is not RulesetEffectSpell spellEffect)
            {
                return true;
            }

            if (__instance.Character.RulesetCharacter is not RulesetCharacterHero hero
                || (SharedSpellsContext.GetWarlockSpellRepertoire(hero) != null
                    && !SharedSpellsContext.IsMulticaster(hero)))
            {
                return true;
            }

            spellEffect.SlotLevel = __instance.SubOptionsAvailability.Keys.ToArray()[option];
            return false;
        }
    }

    [HarmonyPatch(typeof(ReactionRequestCastSpell), nameof(ReactionRequestCastSpell.SelectedSubOption),
        MethodType.Getter)]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class SelectedSubOption_Getter_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ReactionRequestCastSpell __instance, ref int __result)
        {
            //this should always be false
            if (__instance.ReactionParams.RulesetEffect is not RulesetEffectSpell spellEffect)
            {
                return true;
            }

            if (__instance.Character.RulesetCharacter is not RulesetCharacterHero hero
                || (SharedSpellsContext.GetWarlockSpellRepertoire(hero) != null
                    && !SharedSpellsContext.IsMulticaster(hero)))
            {
                return true;
            }

            __result = __instance.SubOptionsAvailability.Keys.ToList().FindIndex(v => v == spellEffect.SlotLevel);

            return false;
        }
    }
}
