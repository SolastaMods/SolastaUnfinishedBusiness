using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private static readonly FeatureDefinitionPower PowerFightingStyleMerciless = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleMerciless")
        .SetGuiPresentationNoContent(true)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create(DatabaseHelper.SpellDefinitions.Fear.EffectDescription)
            .SetSavingThrowData(false,
                AttributeDefinitions.Wisdom, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Strength)
            .SetDurationData(DurationType.Round, 1)
            .Build())
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = FightingStyleBuilder
        .Create("Merciless")
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.SorcerousHauntedSoul)
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
                                         !ValidatorsWeapon.IsUnarmedWeapon(rulesetCharacter, attackMode)))
            {
                yield break;
            }

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var proficiencyBonus = rulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var usablePower = new RulesetUsablePower(PowerFightingStyleMerciless, null, null);
            var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
            var effectPower = new RulesetEffectPower(rulesetCharacter, usablePower);

            foreach (var enemy in battle.EnemyContenders
                         .Where(enemy => attacker.RulesetActor.DistanceTo(enemy.RulesetActor) <= distance))
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
