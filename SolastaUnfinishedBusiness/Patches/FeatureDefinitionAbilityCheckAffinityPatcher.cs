using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class FeatureDefinitionAbilityCheckAffinityPatcher
{
    //BUGFIX: vanilla doesn't handle subtract die on ability check affinity tooltip
    [HarmonyPatch(typeof(FeatureDefinitionAbilityCheckAffinity),
        nameof(FeatureDefinitionAbilityCheckAffinity.FormatDescription))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class FormatDescription_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            FeatureDefinitionAbilityCheckAffinity __instance,
            ref string __result)
        {
            if (!string.IsNullOrEmpty(__instance.GuiPresentation.Description) &&
                __instance.GuiPresentation.Description != Gui.NoLocalization)
            {
                return true;
            }

            __result = string.Empty;

            foreach (var affinityGroup in __instance.affinityGroups)
            {
                if (!string.IsNullOrEmpty(__result))
                {
                    __result += "\n";
                }

                var formatAbilityScoreAndProficiency = Gui.FormatAbilityScoreAndProficiency(
                    affinityGroup.abilityScoreName, affinityGroup.proficiencyName);

                if (affinityGroup.affinity != CharacterAbilityCheckAffinity.None &&
                    affinityGroup.affinity != CharacterAbilityCheckAffinity.AutomaticSuccess)
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (affinityGroup.affinity)
                    {
                        case CharacterAbilityCheckAffinity.Advantage:
                            __result += Gui.FormatWithHighlight(FeatureDefinitionAbilityCheckAffinity.advantageFormat,
                                formatAbilityScoreAndProficiency);
                            break;
                        case CharacterAbilityCheckAffinity.Disadvantage:
                            __result += Gui.FormatWithHighlight(
                                FeatureDefinitionAbilityCheckAffinity.disadvantageFormat,
                                formatAbilityScoreAndProficiency);
                            break;
                        case CharacterAbilityCheckAffinity.AutomaticFail:
                            __result += Gui.FormatWithHighlight(
                                FeatureDefinitionAbilityCheckAffinity.automaticallyFailedFormat,
                                formatAbilityScoreAndProficiency);
                            break;
                    }

                    __result = affinityGroup.abilityCheckContext switch
                    {
                        AbilityCheckContext.EscapingPerception =>
                            __result + " " +
                            Gui.Localize(FeatureDefinitionAbilityCheckAffinity.escapingPerceptionSuffix),
                        AbilityCheckContext.ResistingShove =>
                            __result + " " +
                            Gui.Localize(FeatureDefinitionAbilityCheckAffinity.resistingShoveSuffix),
                        _ => __result
                    };
                }

                if (affinityGroup.abilityCheckModifierDiceNumber == 0)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(__result))
                {
                    __result += "\n";
                }

                if (affinityGroup.abilityCheckModifierDieType == DieType.D1)
                {
                    __result += Gui.FormatWithHighlight(
                        FeatureDefinitionAbilityCheckAffinity.bonusFormat,
                        affinityGroup.abilityCheckModifierDiceNumber.ToString("+0;-#"),
                        formatAbilityScoreAndProficiency);
                }
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                else
                {
                    var diceNumber = affinityGroup.abilityCheckGroupOperation == AbilityCheckGroupOperation.SubstractDie
                        ? -affinityGroup.abilityCheckModifierDiceNumber
                        : affinityGroup.abilityCheckModifierDiceNumber;

                    __result += Gui.FormatWithHighlight(
                        FeatureDefinitionAbilityCheckAffinity.bonusFormatWithDice,
                        diceNumber.ToString("+0;-#"),
                        Gui.FormatDieTitle(affinityGroup.abilityCheckModifierDieType),
                        formatAbilityScoreAndProficiency);
                }
            }

            return false;
        }
    }
}
