using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private static readonly FeatureDefinitionPower PowerFightingStyleMerciless = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleMerciless")
        .SetGuiPresentation("Merciless", Category.FightingStyle)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Cube)
                .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                .SetSavingThrowData(
                    false,
                    AttributeDefinitions.Wisdom,
                    true,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Strength)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                            ConditionForm.ConditionOperation.Add)
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .Build())
                .Build())
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Merciless")
        .SetGuiPresentation(Category.FightingStyle, Sprites.GetSprite("Merciless", Resources.Merciless, 256))
        .SetFeatures(
            FeatureDefinitionBuilder
                .Create("TargetReducedToZeroHpFightingStyleMerciless")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new OnReducedToZeroHpByMeMerciless())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        CharacterContext.FightingStyleChoiceBarbarian,
        CharacterContext.FightingStyleChoiceRogue,
        FightingStyleChampionAdditional,
        FightingStyleFighter,
        FightingStylePaladin,
        FightingStyleRanger
    };

    private sealed class OnReducedToZeroHpByMeMerciless : IOnReducedToZeroHpByMe, IPhysicalAttackFinishedByMe
    {
        private bool _criticalHit;

        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (activeEffect != null || (!ValidatorsWeapon.IsMelee(attackMode)
                                         && !ValidatorsWeapon.IsUnarmed(rulesetAttacker, attackMode)))
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var proficiencyBonus = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var usablePower = UsablePowersProvider.Get(PowerFightingStyleMerciless, rulesetAttacker);
            var distance = _criticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
            var actionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                    .AddAsActivePowerToSource(),
                targetCharacters = gameLocationBattleService.Battle.EnemyContenders
                    .Where(x =>
                        x.IsOppositeSide(attacker.Side)
                        && x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false }
                        && gameLocationBattleService.IsWithinXCells(downedCreature, x, distance))
                    .ToList()
            };

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }

        public IEnumerator OnAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            _criticalHit = attackRollOutcome == RollOutcome.CriticalSuccess;

            yield break;
        }
    }
}
