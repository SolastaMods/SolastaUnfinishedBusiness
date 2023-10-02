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

            //PATCH: supports IFilterTargetingMagicEffect
            foreach (var filterTargetingMagicEffect in
                     definition.GetAllSubFeaturesOfType<IFilterTargetingMagicEffect>())
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
}
