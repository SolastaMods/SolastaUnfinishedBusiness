using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
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
                .SetCustomSubFeatures(new OnReducedToZeroHpEnemyFightingStyleMerciless())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnReducedToZeroHpEnemyFightingStyleMerciless : IOnReducedToZeroHpEnemy
    {
        public IEnumerator HandleReducedToZeroHpEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            // activeEffect != null means a magical attack
            if (activeEffect != null || (!ValidatorsWeapon.IsMelee(attackMode) &&
                                         !ValidatorsWeapon.IsUnarmed(rulesetAttacker, attackMode)))
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService == null)
            {
                yield break;
            }

            var actionParams = Global.CurrentAction.ActionParams.Clone();
            var proficiencyBonus = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            // var strength = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Strength);
            var usablePower = UsablePowersProvider.Get(PowerFightingStyleMerciless, rulesetAttacker);

            //TODO: not sure we need this, since `UsablePowersProvider.Get` already computes DC
            // usablePower.saveDC = ComputeAbilityScoreBasedDC(strength, proficiencyBonus);

            var distance = Global.CurrentAction.AttackRollOutcome == RollOutcome.CriticalSuccess
                ? proficiencyBonus
                : (proficiencyBonus + 1) / 2;

            actionParams.ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower;
            actionParams.RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource();
            actionParams.TargetCharacters.SetRange(gameLocationBattleService.Battle.EnemyContenders
                .Where(x => x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false })
                .Where(enemy => gameLocationBattleService.IsWithinXCells(downedCreature, enemy, distance))
                .ToList());

            Global.CurrentAction.ResultingActions.Add(new CharacterActionSpendPower(actionParams));
        }
    }
}
