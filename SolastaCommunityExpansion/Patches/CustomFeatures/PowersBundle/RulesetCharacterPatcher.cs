using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PowersBundle;

internal static class Helper
{
    internal static void TerminatePowers(RulesetCharacter character, FeatureDefinitionPower exclude,
        IEnumerable<FeatureDefinitionPower> powers)
    {
        var allSubPowers = new HashSet<FeatureDefinitionPower>();

        foreach (var power in powers)
        {
            allSubPowers.Add(power);

            var bundles = PowerBundleContext.GetMasterPowersBySubPower(power);

            foreach (var masterPower in bundles)
            {
                var bundle = PowerBundleContext.GetBundle(masterPower);

                if (!bundle.TerminateAll)
                {
                    continue;
                }

                foreach (var subPower in bundle.SubPowers)
                {
                    allSubPowers.Add(subPower);
                }
            }
        }

        if (exclude != null)
        {
            allSubPowers.Remove(exclude);
        }

        var toTerminate = character.PowersUsedByMe.Where(u => allSubPowers.Contains(u.PowerDefinition)).ToList();
        foreach (var power in toTerminate)
        {
            character.TerminatePower(power);
        }
    }

    internal static void TerminateSpells(RulesetCharacter character, SpellDefinition exclude,
        IEnumerable<SpellDefinition> spells)
    {
        var allSubSpells = new HashSet<SpellDefinition>();

        foreach (var spell in spells)
        {
            allSubSpells.Add(spell);
            foreach (var allElement in DatabaseRepository.GetDatabase<SpellDefinition>().GetAllElements())
            {
                if (!spell.IsSubSpellOf(allElement))
                {
                    continue;
                }

                foreach (var subSpell in allElement.SubspellsList)
                {
                    allSubSpells.Add(subSpell);
                }
            }
        }

        if (exclude != null)
        {
            allSubSpells.Remove(exclude);
        }

        var toTerminate = character.SpellsCastByMe.Where(c => allSubSpells.Contains(c.SpellDefinition)).ToList();
        foreach (var spell in toTerminate)
        {
            character.TerminateSpell(spell);
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniquePower")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetCharacter_TerminateMatchingUniquePower
{
    internal static void Postfix(RulesetCharacter __instance, FeatureDefinitionPower powerDefinition)
    {
        var (powers, spells) = GlobalUniqueEffects.GetSameGroupItems(powerDefinition);

        powers.Add(powerDefinition);
        Helper.TerminatePowers(__instance, powerDefinition, powers);
        Helper.TerminateSpells(__instance, null, spells);
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "TerminateMatchingUniqueSpell")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_TerminateMatchingUniqueSpell
{
    internal static void Postfix(RulesetCharacter __instance, SpellDefinition spellDefinition)
    {
        var (powers, spells) = GlobalUniqueEffects.GetSameGroupItems(spellDefinition);

        spells.Add(spellDefinition);
        Helper.TerminatePowers(__instance, null, powers);
        Helper.TerminateSpells(__instance, spellDefinition, spells);
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "OnConditionAdded")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_OnConditionAdded
{
    internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
    {
        var features = activeCondition.ConditionDefinition.Features.ToList();
        foreach (var feature in features)
        {
            if (feature is ICustomConditionFeature custom)
            {
                custom.ApplyFeature(__instance);
            }
        }
    }
}

[HarmonyPatch(typeof(RulesetCharacter), "OnConditionRemoved")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class RulesetActor_OnConditionRemoved
{
    internal static void Postfix(RulesetCharacter __instance, RulesetCondition activeCondition)
    {
        var features = activeCondition.ConditionDefinition.Features.ToList();
        foreach (var feature in features)
        {
            if (feature is ICustomConditionFeature custom)
            {
                custom.RemoveFeature(__instance);
            }
        }
    }
}
