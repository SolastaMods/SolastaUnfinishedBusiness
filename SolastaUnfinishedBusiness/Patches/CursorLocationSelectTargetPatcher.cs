using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Spells;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Subclasses.SorcerousFieldManipulator;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationSelectTargetPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.IsFilteringValid))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class IsFilteringValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target,
            ref bool __result)
        {
            var definition = __instance.ActionParams.activeEffect.SourceDefinition;

            //PATCH: supports IFilterTargetingCharacter
            foreach (var filterTargetingMagicEffect in
                     definition.GetAllSubFeaturesOfType<IFilterTargetingCharacter>())
            {
                __result = filterTargetingMagicEffect.IsValid(__instance, target);

                if (__result)
                {
                    return;
                }
            }

            //PATCH: supports Find Familiar specific case for any caster as spell can be granted to other classes
            if (__instance.actionParams.RulesetEffect is RulesetEffectSpell rulesetEffectSpell &&
                rulesetEffectSpell.EffectDescription.RangeType is RangeType.Touch or RangeType.MeleeHit)
            {
                var rulesetCharacter = __instance.actionParams.actingCharacter.RulesetCharacter;
                var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

                if (rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                    gameLocationBattleService is { IsBattleInProgress: true })
                {
                    var familiar = gameLocationBattleService.Battle.PlayerContenders
                        .FirstOrDefault(x =>
                            x.RulesetCharacter is RulesetCharacterMonster rulesetCharacterMonster &&
                            rulesetCharacterMonster.MonsterDefinition.Name == SpellBuilders.OwlFamiliar &&
                            rulesetCharacterMonster.AllConditions.Exists(y =>
                                y.ConditionDefinition == ConditionDefinitions.ConditionConjuredCreature &&
                                y.SourceGuid == rulesetCharacter.Guid));

                    var canAttack = familiar != null && gameLocationBattleService.IsWithin1Cell(familiar, target);

                    if (canAttack)
                    {
                        var effectDescription = new EffectDescription();

                        effectDescription.Copy(__instance.effectDescription);
                        effectDescription.rangeType = RangeType.RangeHit;
                        effectDescription.rangeParameter = 24;

                        __instance.effectDescription = effectDescription;
                    }
                    else
                    {
                        __instance.effectDescription = __instance.ActionParams.RulesetEffect.EffectDescription;
                    }
                }
            }

            //PATCH: support for target spell filtering based on custom spell filters
            // used for melee cantrips to limit targets to weapon attack range
            if (!__result)
            {
                return;
            }

            __result = IsFilteringValidMeleeCantrip(__instance, target);
        }

        private static bool IsFilteringValidMeleeCantrip(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target)
        {
            var actionParams = __instance.actionParams;
            var canBeUsedToAttack = actionParams?.RulesetEffect
                ?.SourceDefinition.GetFirstSubFeatureOfType<IAttackAfterMagicEffect>()?.CanBeUsedToAttack;

            if (canBeUsedToAttack == null || canBeUsedToAttack(__instance, actionParams.actingCharacter, target,
                    out var failure))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add(failure);

            return false;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.Activate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Activate_Patch
    {
        [UsedImplicitly]
        public static void Prefix(params object[] parameters)
        {
            //PATCH: allows Sorcerous Field Manipulator displacement to select any character
            if (parameters.Length > 0 &&
                parameters[0] is CharacterActionParams
                {
                    RulesetEffect: RulesetEffectPower rulesetEffectPower
                } characterActionParams &&
                rulesetEffectPower.PowerDefinition == PowerSorcerousFieldManipulatorDisplacement)
            {
                // allows any target to be selected as well as automatically presents a better UI description
                characterActionParams.RulesetEffect.EffectDescription.inviteOptionalAlly = false;
            }
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.Deactivate))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Deactivate_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CursorLocationSelectTarget __instance)
        {
            //PATCH: allows Sorcerous Field Manipulator displacement to select any character
            if (__instance.actionParams is { RulesetEffect: RulesetEffectPower rulesetEffectPower } &&
                rulesetEffectPower.PowerDefinition == PowerSorcerousFieldManipulatorDisplacement)
            {
                // brings back power effect to it's original definition
                rulesetEffectPower.EffectDescription.inviteOptionalAlly = true;
            }
        }
    }

    //PATCH: support EnforceFullSelection on IFilterTargetingCharacter (vanilla code except for patch block)
    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.RefreshCaption))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshCaption_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CursorLocationSelectTarget __instance)
        {
            if (CursorLocation.CaptionLineChanged == null)
            {
                return false;
            }

            var captionCounter = string.Empty;
            var str = string.Empty;

            if (__instance.effectDescription != null)
            {
                if (__instance.effectDescription.TargetType == TargetType.Position &&
                    __instance.effectDescription.InviteOptionalAlly)
                {
                    CursorLocation.CaptionLineChanged(__instance.captionTitle,
                        Gui.Localize("Caption/&InviteOptionalAllyCaption"), captionCounter, string.Empty, string.Empty,
                        string.Empty, true, true);

                    return false;
                }

                if (__instance.effectDescription.TargetFilteringMethod is TargetFilteringMethod.AllCharacterAndGadgets
                    or TargetFilteringMethod.CharacterOnly or TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    switch (__instance.TargetSide)
                    {
                        case Side.All:
                            str += Gui.Localize("Caption/&TargetFilteringCreature");
                            break;
                        case Side.Ally:
                            str += Gui.Localize("Caption/&TargetFilteringAllyCreature");
                            break;
                        case Side.Enemy:
                            str += Gui.Localize("Caption/&TargetFilteringEnemyCreature");
                            break;
                    }
                }

                if (__instance.effectDescription.TargetFilteringMethod is TargetFilteringMethod.AllCharacterAndGadgets
                    or TargetFilteringMethod.GadgetOnly or TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += Gui.ListSeparator();
                    }

                    str += Gui.Localize("Caption/&TargetFilteringGadget");
                }

                if (__instance.effectDescription.TargetFilteringMethod ==
                    TargetFilteringMethod.CharacterGadgetEffectProxy)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        str += Gui.ListSeparator();
                    }

                    str += Gui.Localize("Caption/&TargetFilteringEffectProxy");
                }
            }
            else
            {
                str = __instance.TargetSide != Side.Ally
                    ? str + Gui.Localize("Caption/&TargetFilteringCreature")
                    : str + Gui.Localize("Caption/&TargetFilteringAllyCreature");
            }

            string captionContent;
            if (__instance.effectDescription is { TargetType: TargetType.SharedAmongIndividuals })
            {
                captionContent = Gui.Format("Caption/&TargetShareUniqueCaption", str,
                    __instance.GameLocationSelectionService.SelectedTargets.Count.ToString());
            }
            else
            {
                string baseText;

                if (__instance.maxTargets == 1)
                {
                    baseText = "Caption/&TargetSingleCaption";
                }
                else
                {
                    baseText = !__instance.uniqueTargets
                        ? "Caption/&TargetMultipleCaption"
                        : "Caption/&TargetMultipleUniqueCaption";
                    captionCounter = Gui.FormatCurrentOverMax(__instance.maxTargets - __instance.remainingTargets,
                        __instance.maxTargets);
                }

                captionContent = Gui.Format(baseText, str, __instance.remainingTargets.ToString());
            }

            var captionProximity = string.Empty;

            if (__instance.effectDescription is { RequiresTargetProximity: true, TargetProximityDistance: > 0 })
            {
                captionProximity = Gui.Format(
                    __instance.effectDescription.TargetProximityDistance == 1
                        ? "Caption/&TargetProximitySingleCaption"
                        : "Caption/&TargetProximityMultipleCaption",
                    __instance.effectDescription.TargetProximityDistance.ToString());
            }

            if (__instance.effectDescription is { TargetType: TargetType.ArcFromIndividual })
            {
                var targetParameter = __instance.ActionParams.RulesetEffect.ComputeTargetParameter();
                var targetParameter2 = __instance.ActionParams.RulesetEffect.ComputeTargetParameter2();

                captionProximity +=
                    Gui.Format(
                        targetParameter <= 1
                            ? "Caption/&TargetArcSubtargetSingleCaption"
                            : "Caption/&TargetArcSubtargetMultipleCaption", targetParameter.ToString(),
                        targetParameter2.ToString());
            }

            var captionRequiredCondition = string.Empty;

            if (__instance.effectDescription != null && __instance.effectDescription.TargetCondition != null)
            {
                captionRequiredCondition = Gui.Format("Caption/&TargetRequiredConditionCaption",
                    __instance.effectDescription.TargetCondition.GuiPresentation.Title);
            }

            if (__instance.effectDescription is { RestrictedCreatureFamilies.Count: > 0 })
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Format("Caption/&TargetRequiredCreatureTypeCaption",
                    __instance.effectDescription.EnumerateRestrictedCreatureFamilies());
            }

            if (__instance.effectDescription != null &&
                (__instance.effectDescription.TargetFilteringTag & TargetFilteringTag.Unarmored) !=
                TargetFilteringTag.No)
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Localize("Caption/&TargetRequiredUnarmoredCaption");
            }

            if (__instance.effectDescription != null &&
                (__instance.effectDescription.TargetFilteringTag & TargetFilteringTag.MetalArmor) !=
                TargetFilteringTag.No)
            {
                if (!string.IsNullOrEmpty(captionRequiredCondition))
                {
                    captionRequiredCondition += " ";
                }

                captionRequiredCondition += Gui.Localize("Caption/&TargetRequiredMetalArmorCaption");
            }

            var creatureSizeCaption =
                CursorLocationSelectTarget.GenerateCreatureSizeCaption(__instance.effectDescription);

            // BEGIN PATCH
            // var canProceed = __instance.maxTargets < 0 || (__instance.maxTargets > 1 && __instance.remainingTargets < __instance.maxTargets);

            bool canProceed;
            var enforceFullSelection = false;

            if (__instance.actionParams is { RulesetEffect: RulesetEffectPower rulesetEffectPower })
            {
                var modifier = rulesetEffectPower.PowerDefinition.GetFirstSubFeatureOfType<IFilterTargetingCharacter>();

                if (modifier != null)
                {
                    enforceFullSelection = modifier.EnforceFullSelection;
                }
            }

            if (enforceFullSelection)
            {
                canProceed = __instance.maxTargets < 0 ||
                             (__instance.maxTargets > 1 && __instance.remainingTargets == 0);
            }
            else
            {
                canProceed = __instance.maxTargets < 0 ||
                             (__instance.maxTargets > 1 && __instance.remainingTargets < __instance.maxTargets);
            }
            // END PATCH

            CursorLocation.CaptionLineChanged(__instance.captionTitle, captionContent, captionCounter, captionProximity,
                captionRequiredCondition, creatureSizeCaption, canProceed, true);

            return false;
        }
    }
}
