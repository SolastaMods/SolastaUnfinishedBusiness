using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures
{
    //
    // Power Related Patches
    //
    [HarmonyPatch(typeof(RulesetCharacter), "UsePower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_UsePower
    {
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            __instance.UpdateUsageForPowerPool(usablePower, usablePower.PowerDefinition.CostPerUse);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "RepayPowerUse")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_RepayPowerUse
    {
        public static void Postfix(RulesetCharacter __instance, RulesetUsablePower usablePower)
        {
            __instance.UpdateUsageForPowerPool(usablePower, -usablePower.PowerDefinition.CostPerUse);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "GrantPowers")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_GrantPowers
    {
        public static void Postfix(RulesetCharacter __instance)
        {
            CustomFeaturesContext.RechargeLinkedPowers(__instance, RuleDefinitions.RestType.LongRest);
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "ApplyRest")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_ApplyRest
    {
        internal static void Postfix(
            RulesetCharacter __instance, RuleDefinitions.RestType restType, bool simulate)
        {
            if (!simulate)
            {
                CustomFeaturesContext.RechargeLinkedPowers(__instance, restType);
            }

            // The player isn't recharging the shared pool features, just the pool.
            // Hide the features that use the pool from the UI.
            foreach (FeatureDefinition feature in __instance.RecoveredFeatures.Where(f => f is IPowerSharedPool).ToArray())
            {
                __instance.RecoveredFeatures.Remove(feature);
            }
        }
    }

    [HarmonyPatch(typeof(RulesetCharacter), "ComputeAutopreparedSpells")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetCharacter_ComputeAutopreparedSpells_Patch
    {
        internal static bool Prefix(RulesetCharacter __instance, RulesetSpellRepertoire spellRepertoire)
        {
            if (!Main.Settings.SupportAutoPreparedSpellsOnSubclassCasters)
            {
                return true;
            }

            CharacterClassDefinition spellcastingClass = spellRepertoire.SpellCastingClass;
            if (spellRepertoire.SpellCastingSubclass != null)
            {
                spellcastingClass = GetClassForSubclass(spellRepertoire.SpellCastingSubclass);
            }

            spellRepertoire.AutoPreparedSpells.Clear();
            __instance.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(__instance.FeaturesToBrowse);
            foreach (FeatureDefinition featureDefinition in __instance.FeaturesToBrowse)
            {
                FeatureDefinitionAutoPreparedSpells autoPreparedSpells = featureDefinition as FeatureDefinitionAutoPreparedSpells;
                if (autoPreparedSpells.SpellcastingClass == spellcastingClass)
                {
                    foreach (FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups)
                    {
                        if (preparedSpellsGroup.ClassLevel <= GetSpellcastingLevel(__instance, spellRepertoire))
                        {
                            spellRepertoire.AutoPreparedSpells.AddRange(preparedSpellsGroup.SpellsList);
                            spellRepertoire.AutoPreparedTag = autoPreparedSpells.AutoPreparedTag;
                        }
                    }
                }
            }
            // This includes all the logic for the base function and a little extra, so skip it.
            return false;
        }

        private static int GetSpellcastingLevel(RulesetCharacter character, RulesetSpellRepertoire spellRepertoire)
        {
            if (character is RulesetCharacterHero hero)
            {
                if (spellRepertoire.SpellCastingClass != null)
                {
                    return hero.ClassesAndLevels[spellRepertoire.SpellCastingClass];
                }
                if (spellRepertoire.SpellCastingSubclass != null)
                {
                    return hero.ComputeSubclassLevel(spellRepertoire.SpellCastingSubclass);
                }
            }
            return character.GetAttribute(AttributeDefinitions.CharacterLevel).BaseValue;
        }

        private static CharacterClassDefinition GetClassForSubclass(CharacterSubclassDefinition subclass)
        {
            return DatabaseRepository.GetDatabase<CharacterClassDefinition>().FirstOrDefault(klass =>
            {
                return klass.FeatureUnlocks.Any(unlock =>
                {
                    if (unlock.FeatureDefinition is FeatureDefinitionSubclassChoice subclassChoice)
                    {
                        return subclassChoice.Subclasses.Contains(subclass.Name);
                    }
                    return false;
                });
            });
        }
    }
}
