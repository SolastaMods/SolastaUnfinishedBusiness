using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomDefinitions;
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
            FeatureDefinitionOnCharacterKillBuilder
                .Create("OnCharacterKillFightingStyleMerciless")
                .SetGuiPresentationNoContent()
                .SetOnCharacterKill(character =>
                {
                    if (Global.CurrentAction is not CharacterActionAttack actionAttack)
                    {
                        return;
                    }

                    var battle = ServiceRepository.GetService<IGameLocationBattleService>()?.Battle;

                    if (battle == null)
                    {
                        return;
                    }

                    var attacker = actionAttack.ActingCharacter.RulesetCharacter as RulesetCharacterHero
                                   ?? actionAttack.ActingCharacter.RulesetCharacter.OriginalFormCharacter as
                                       RulesetCharacterHero;

                    if (attacker == null || attacker.IsWieldingRangedWeapon())
                    {
                        return;
                    }

                    var proficiencyBonus = attacker.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
                    var strength = attacker.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
                    var distance = Global.CriticalHit ? proficiencyBonus : (proficiencyBonus + 1) / 2;
                    var usablePower = new RulesetUsablePower(
                        PowerFightingStyleMerciless, attacker.RaceDefinition, attacker.ClassesHistory[0]);
                    var effectPower = new RulesetEffectPower(attacker, usablePower);

                    usablePower.SaveDC = 8 + proficiencyBonus
                                           + AttributeDefinitions.ComputeAbilityScoreModifier(strength);

                    foreach (var enemy in battle.EnemyContenders
                                 .Where(enemy =>
                                     enemy != character &&
                                     int3.Distance(character.LocationPosition, enemy.LocationPosition) <= distance))
                    {
                        effectPower.ApplyEffectOnCharacter(enemy.RulesetCharacter, true, enemy.LocationPosition);
                    }
                })
                .AddToDB())
        .AddToDB();

    internal override List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice => new()
    {
        FightingStyleChampionAdditional, FightingStyleFighter, FightingStylePaladin, FightingStyleRanger
    };
}
