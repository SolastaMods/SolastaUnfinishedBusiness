using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using TA;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFightingStyleChoices;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal sealed class Merciless : AbstractFightingStyle
{
    private static readonly FeatureDefinitionPower PowerFightingStyleMerciless = FeatureDefinitionPowerBuilder
        .Create("PowerFightingStyleMerciless")
        .SetGuiPresentation("Fear", Category.Spell)
        .SetUsesProficiencyBonus(ActivationTime.NoCost)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create(DatabaseHelper.SpellDefinitions.Fear.EffectDescription)
            .SetDurationData(DurationType.Round, 1)
            .SetTargetingData(Side.All, RangeType.Self, 6, TargetType.IndividualsUnique)
            .Build())
        .AddToDB();

    internal override FightingStyleDefinition FightingStyle { get; } = CustomizableFightingStyleBuilder
        .Create("Merciless")
        .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.CharacterSubclassDefinitions.MartialChampion)
        .SetFeatures(
            // FeatureDefinitionAdditionalActionBuilder
            //     .Create(AdditionalActionHunterHordeBreaker, "AdditionalActionFightingStyleMerciless")
            //     .SetGuiPresentationNoContent()
            //     .AddToDB(),
            FeatureDefinitionBuilder
                .Create("OnCharacterKillFightingStyleMerciless")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(new OnCharacterKillFightingStyleMerciless())
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };

    private sealed class OnCharacterKillFightingStyleMerciless : ITargetReducedToZeroHp
    {
        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (Global.CurrentAction is not CharacterActionAttack)
            {
                yield break;
            }

            var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

            if (battle == null)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter as RulesetCharacterHero ??
                                  attacker.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero;

            if (rulesetAttacker == null || rulesetAttacker.IsWieldingRangedWeapon())
            {
                yield break;
            }

            var proficiencyBonus = rulesetAttacker.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var strength = rulesetAttacker.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
            var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
            var usablePower = new RulesetUsablePower(PowerFightingStyleMerciless, rulesetAttacker.RaceDefinition,
                rulesetAttacker.ClassesHistory[0]);
            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);

            usablePower.SaveDC = 8 + proficiencyBonus + AttributeDefinitions.ComputeAbilityScoreModifier(strength);

            foreach (var enemy in battle.EnemyContenders
                         .Where(enemy =>
                             enemy != attacker && int3.Distance(attacker.LocationPosition, enemy.LocationPosition) <=
                             distance))
            {
                effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
            }
        }
    }
}
