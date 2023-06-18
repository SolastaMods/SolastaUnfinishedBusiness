using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
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
        .SetEffectDescription(EffectDescriptionBuilder.Create()
            .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Cube)
            .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSavingThrowData(
                false,
                AttributeDefinitions.Wisdom,
                true,
                EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Strength)
            .SetEffectForms(EffectFormBuilder.Create()
                .SetConditionForm(ConditionDefinitions.ConditionFrightenedFear, ConditionForm.ConditionOperation.Add)
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
                .SetCustomSubFeatures(new TargetReducedToZeroHpFightingStyleMerciless())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class TargetReducedToZeroHpFightingStyleMerciless :
        ITargetReducedToZeroHp, IAttackBeforeHitConfirmedOnEnemy
    {
        private bool isCritical;

        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            isCritical = criticalHit;

            yield break;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            // activeEffect != null means a magical attack
            if (activeEffect != null || (!ValidatorsWeapon.IsMelee(attackMode) &&
                                         !ValidatorsWeapon.IsUnarmed(rulesetCharacter, attackMode)))
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService == null)
            {
                yield break;
            }

            var proficiencyBonus = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var strength = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Strength);
            var usablePower = UsablePowersProvider.Get(PowerFightingStyleMerciless, rulesetCharacter);

            //TODO: not sure we need this, since `UsablePowersProvider.Get` already computes DC
            usablePower.saveDC = ComputeAbilityScoreBasedDC(strength, proficiencyBonus);

            var effectPower = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetCharacter, usablePower, false)
                .AddAsActivePowerToSource();

            var distance = isCritical ? proficiencyBonus : (proficiencyBonus + 1) / 2;

            effectPower.EffectDescription.targetParameter = (distance * 2) + 1;

            foreach (var enemy in gameLocationBattleService.Battle.EnemyContenders
                         .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                         .Where(enemy => gameLocationBattleService.IsWithinXCells(downedCreature, enemy, distance))
                         .ToList()) // avoid changing enumerator
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }

            isCritical = false;
        }
    }
}
