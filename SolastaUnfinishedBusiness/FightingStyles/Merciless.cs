using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private static readonly FeatureDefinitionPower PowerFightingStyleMerciless = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleMerciless")
        .SetGuiPresentation("Merciless", Category.FightingStyle)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create(DatabaseHelper.SpellDefinitions.Fear.EffectDescription)
            .SetTargetingData(Side.Enemy, RangeType.Touch, 1, TargetType.Cube)
            .SetSavingThrowData(false,
                AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Strength)
            .SetDurationData(DurationType.Round, 1)
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

    private sealed class TargetReducedToZeroHpFightingStyleMerciless : ITargetReducedToZeroHp
    {
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

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var proficiencyBonus = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var strength = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Strength);
            var usablePower = new RulesetUsablePower(PowerFightingStyleMerciless, null, null)
            {
                saveDC = ComputeAbilityScoreBasedDC(strength, proficiencyBonus)
            };
            var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower)
            {
                EffectDescription = { targetParameter = (distance * 2) + 1 }
            };

            foreach (var enemy in battle.EnemyContenders
                         .Where(enemy => downedCreature.RulesetActor.DistanceTo(enemy.RulesetActor) <= distance))
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
