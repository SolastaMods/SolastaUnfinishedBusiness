using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.CustomDefinitions;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.Patches;

internal static class RulesetImplementationManagerLocationPatcher
{
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsMetamagicOptionAvailable_Patch
    {
        //TODO: improve sunlight blade so it can properly work when twinned
        private static readonly string[] NotAllowedSpells = {"SunlightBlade"};

        internal static void Postfix(
            ref bool __result,
            RulesetEffectSpell rulesetEffectSpell,
            RulesetCharacter caster,
            MetamagicOptionDefinition metamagicOption,
            ref string failure)
        {
            //PATCH: fix twinned spells offering
            //disables sunlight blade twinning, since it is not supported for now
            //plus fixes vanilla code not accounting for things possible in MC
            if (metamagicOption != DatabaseHelper.MetamagicOptionDefinitions.MetamagicTwinnedSpell
                || caster is not RulesetCharacterHero hero)
            {
                return;
            }

            var spellDefinition = rulesetEffectSpell.SpellDefinition;

            if (Array.IndexOf(NotAllowedSpells, spellDefinition.Name) >= 0)
            {
                failure = "Cannot be twinned";
                __result = false;

                return;
            }

            if (!Main.Settings.FixSorcererTwinnedLogic)
            {
                return;
            }

            var effectDescription = spellDefinition.effectDescription;
            if (effectDescription.TargetType is TargetType.Individuals or TargetType.IndividualsUnique)
            {
                if (rulesetEffectSpell.ComputeTargetParameter() != 1)
                {
                    failure = FailureFlagInvalidSingleTarget;
                    __result = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsSituationalContextValid")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsSituationalContextValid_Patch
    {
        internal static void Postfix(
            ref bool __result,
            RulesetImplementationDefinitions.SituationalContextParams contextParams)
        {
            //PATCH: supports custom situational context
            //used to tweak `Reckless` feat to properly work with reach weapons
            //and for Blade Dancer subclass features
            __result = CustomSituationalContext.IsContextValid(contextParams, __result);
        }
    }
}
